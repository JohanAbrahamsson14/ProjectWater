using UnityEngine;
using System.Collections.Generic;

public class TrackFollower : MonoBehaviour
{
    public float baseRotationSpeed = 1f;
    public float maxTiltAngle = 30f; // Maximum tilt angle for the trolley
    public float tiltDamping = 5f; // Damping factor for smoothing the tilt

    private TrolleyController trolleyController;
    private List<TrackSegment> activeSegments = new List<TrackSegment>();
    private int currentSegmentIndex = 0;
    private int currentPathIndex = 0;
    private float traveledDistance = 0f;
    private float segmentLength = 0f;
    private float currentTiltAngle = 0f; // Current tilt angle for smoothing

    void Start()
    {
        trolleyController = GetComponent<TrolleyController>();
    }

    void Update()
    {
        if (activeSegments.Count == 0 || activeSegments[currentPathIndex].keyPoints.Count < 2)
            return;

        MoveAlongCurve();
    }

    public void AddTrackSegment(TrackSegment segment)
    {
        activeSegments.Add(segment);
        if (activeSegments.Count == 1)
        {
            InitializeTrackSegment(0);
        }
    }

    void InitializeTrackSegment(int pathIndex)
    {
        currentPathIndex = pathIndex;
        currentSegmentIndex = 0;
        traveledDistance = 0f;
        CalculateSegmentLength();
    }

    void MoveAlongCurve()
    {
        TrackSegment currentSegment = activeSegments[currentPathIndex];
        float speed = trolleyController.CurrentSpeed;
        traveledDistance += speed * Time.deltaTime;

        // Calculate t based on traveled distance and segment length
        float t = traveledDistance / segmentLength;

        // If t is greater than 1, move to the next segment
        if (t > 1f)
        {
            traveledDistance -= segmentLength; // Reset traveled distance for the next segment
            currentSegmentIndex += 1;
            if (currentSegmentIndex >= currentSegment.keyPoints.Count - 1)
            {
                CheckForIntersection();
            }
            CalculateSegmentLength();
            t = traveledDistance / segmentLength; // Recalculate t for the new segment
        }

        Vector3 p0 = currentSegment.keyPoints[currentSegmentIndex].position;
        Vector3 p1 = currentSegment.controlPoints[currentSegmentIndex].position;
        Vector3 p2 = currentSegment.keyPoints[currentSegmentIndex + 1].position;

        // Get the next position on the curve
        Vector3 nextPosition = BezierCurve.GetPoint(p0, p1, p2, t);
        Vector3 tangent = BezierCurve.GetTangent(p0, p1, p2, t);

        // Move towards the next position
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);

        // Smoothly rotate towards the tangent direction
        Quaternion targetRotation = Quaternion.LookRotation(tangent);
        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        float rotationSpeed = baseRotationSpeed * (speed / trolleyController.maxSpeed) * (angle / 180f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Calculate the z-rotation for tilt effect based on speed and curve angle
        float curveAngle = Vector3.SignedAngle(transform.forward, tangent, Vector3.up);
        float targetTiltAngle = Mathf.Clamp(-curveAngle, -maxTiltAngle, maxTiltAngle) * (speed / trolleyController.maxSpeed);

        // Smoothly interpolate the current tilt angle
        currentTiltAngle = Mathf.Lerp(currentTiltAngle, targetTiltAngle, Time.deltaTime * tiltDamping);
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, currentTiltAngle);
    }

    void CalculateSegmentLength()
    {
        TrackSegment currentSegment = activeSegments[currentPathIndex];
        if (currentSegment.keyPoints.Count < 2 || currentSegment.controlPoints.Count < 1)
        {
            Debug.LogError("Not enough points in the current segment.");
            return;
        }

        Vector3 p0 = currentSegment.keyPoints[currentSegmentIndex].position;
        Vector3 p1 = currentSegment.controlPoints[currentSegmentIndex].position;
        Vector3 p2 = currentSegment.keyPoints[currentSegmentIndex + 1].position;
        segmentLength = BezierCurve.GetApproximateLength(p0, p1, p2);
    }

    void CheckForIntersection()
    {
        // Detect if the current key point is an intersection
        TrackIntersection intersection = activeSegments[currentPathIndex].keyPoints[currentSegmentIndex].GetComponent<TrackIntersection>();
        if (intersection != null)
        {
            // Example: Switch to path 2 if Space is pressed
            bool switchToPath2 = Input.GetKeyDown(KeyCode.Space);

            if (switchToPath2)
            {
                AddTrackSegment(intersection.path2Segment);
            }
            else
            {
                AddTrackSegment(intersection.path1Segment);
            }

            InitializeTrackSegment(activeSegments.Count - 1);
        }
    }
}
