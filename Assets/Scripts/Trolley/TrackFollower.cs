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
    private int currentKeyPointIndex = 0;
    private float traveledDistance = 0f;
    private float segmentLength = 0f;
    private float currentTiltAngle = 0f; // Current tilt angle for smoothing

    void Start()
    {
        trolleyController = GetComponent<TrolleyController>();
    }

    void Update()
    {
        if (activeSegments.Count == 0 || activeSegments[0].keyPoints.Count < 2)
            return;

        MoveAlongCurve();
    }

    public void InitializeTrack(TrackSegment initialSegment)
    {
        activeSegments.Clear();
        AddTrackSegment(initialSegment);
    }

    void AddTrackSegment(TrackSegment segment)
    {
        activeSegments.Add(segment);
        if (activeSegments.Count == 1)
        {
            InitializeTrackSegment(0);
        }
    }

    void InitializeTrackSegment(int segmentIndex)
    {
        currentSegmentIndex = segmentIndex;
        currentKeyPointIndex = 0;
        traveledDistance = 0f;
        CalculateSegmentLength();
    }

    void MoveAlongCurve()
    {
        TrackSegment currentSegment = activeSegments[currentSegmentIndex];
        float speed = trolleyController.CurrentSpeed;
        traveledDistance += speed * Time.deltaTime;

        // Calculate t based on traveled distance and segment length
        float t = traveledDistance / segmentLength;

        // If t is greater than 1, move to the next key point
        if (t > 1f)
        {
            traveledDistance -= segmentLength; // Reset traveled distance for the next segment
            currentKeyPointIndex += 1;

            // Check if we need to move to the next segment or handle intersection
            if (currentKeyPointIndex >= currentSegment.keyPoints.Count - 1)
            {
                // Handle intersection
                if (currentSegment.intersection != null)
                {
                    CheckForIntersection(currentSegment.intersection);
                }
                else if (currentSegment.nextSegment != null)
                {
                    AddTrackSegment(currentSegment.nextSegment);
                    InitializeTrackSegment(activeSegments.Count - 1);
                }
                else
                {
                    return; // End of track
                }
            }
            CalculateSegmentLength();
            t = traveledDistance / segmentLength; // Recalculate t for the new segment
        }

        if (currentKeyPointIndex < currentSegment.keyPoints.Count - 1)
        {
            Vector3 p0 = currentSegment.keyPoints[currentKeyPointIndex].position;
            Vector3 p1 = currentSegment.controlPoints[currentKeyPointIndex].position;
            Vector3 p2 = currentSegment.keyPoints[currentKeyPointIndex + 1].position;

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
    }

    void CalculateSegmentLength()
    {
        TrackSegment currentSegment = activeSegments[currentSegmentIndex];
        if (currentKeyPointIndex < currentSegment.keyPoints.Count - 1 && currentSegment.controlPoints.Count > currentKeyPointIndex)
        {
            Vector3 p0 = currentSegment.keyPoints[currentKeyPointIndex].position;
            Vector3 p1 = currentSegment.controlPoints[currentKeyPointIndex].position;
            Vector3 p2 = currentSegment.keyPoints[currentKeyPointIndex + 1].position;
            segmentLength = BezierCurve.GetApproximateLength(p0, p1, p2);
        }
        else
        {
            segmentLength = 0f;
        }
    }

    void CheckForIntersection(TrackIntersection intersection)
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
