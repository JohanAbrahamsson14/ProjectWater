using UnityEngine;
using System.Collections.Generic;

public class TrackFollower : MonoBehaviour
{
    public float baseRotationSpeed = 1f;
    public float maxTiltAngle = 45f; // Maximum tilt angle for the trolley
    public float tiltDamping = 2f; // Damping factor for smoothing the tilt
    public float tiltMultiplier = 2f; // Multiplier to amplify the tilt effect
    public float chaosMultiplier = 0.1f; // Base multiplier for chaotic shaking effect
    public float maxChaosAngle = 5f; // Maximum angle deviation for chaotic shaking

    private TrolleyController trolleyController;
    private List<TrackSegment> activeSegments = new List<TrackSegment>();
    private int currentSegmentIndex = 0;
    private int currentKeyPointIndex = 0;
    private float traveledDistance = 0f;
    private float segmentLength = 0f;
    private float currentTiltAngle = 0f; // Current tilt angle for smoothing
    private float previousTiltAngle = 0f; // Previous tilt angle for smooth transitions
    private Quaternion previousRotation; // Previous rotation for smooth transitions
    private bool isTurning = false; // Flag to indicate if the trolley is turning
    private float chaosOffsetX;
    private float chaosOffsetY;
    private float chaosOffsetZ;
    private float chaosRotationOffset;

    void Start()
    {
        trolleyController = GetComponent<TrolleyController>();
        chaosOffsetX = Random.Range(0f, 100f);
        chaosOffsetY = Random.Range(0f, 100f);
        chaosOffsetZ = Random.Range(0f, 100f);
        chaosRotationOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (activeSegments.Count == 0 || activeSegments[0].keyPoints.Count < 2)
            return;

        MoveAlongCurve();
        ApplyChaosShaking();
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
        previousRotation = transform.rotation; // Initialize previous rotation
        previousTiltAngle = currentTiltAngle; // Initialize previous tilt angle
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

            // Determine if we are turning
            isTurning = Vector3.SignedAngle(transform.forward, tangent, Vector3.up) != 0;

            // Smoothly rotate towards the tangent direction
            Quaternion targetRotation = Quaternion.LookRotation(tangent);
            float rotationSpeed = baseRotationSpeed * (speed / trolleyController.maxSpeed);

            // Handle smooth rotation transitions at segment boundaries
            if (t < 0.1f) // Smooth rotation at the start of the segment
            {
                transform.rotation = Quaternion.Slerp(previousRotation, targetRotation, t * 10f);
            }
            else if (t > 0.9f) // Smooth rotation at the end of the segment
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, (t - 0.9f) * 10f);
            }
            else // Normal rotation within the segment
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Calculate the z-rotation for tilt effect based on speed and curve angle
            float curveAngle = Vector3.SignedAngle(transform.forward, tangent, Vector3.up);
            float targetTiltAngle = Mathf.Clamp(-curveAngle * tiltMultiplier, -maxTiltAngle, maxTiltAngle) * (speed / trolleyController.maxSpeed);
            
            // Smoothly interpolate the current tilt angle
            currentTiltAngle = Mathf.Lerp(currentTiltAngle, targetTiltAngle, Time.deltaTime * tiltDamping);
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, currentTiltAngle);
            previousRotation = transform.rotation; // Update previous rotation
            previousTiltAngle = currentTiltAngle; // Update previous tilt angle
        }
    }

    void ApplyChaosShaking()
    {
        float speed = trolleyController.CurrentSpeed; // Get current speed of the trolley
        float speedFactor = speed / trolleyController.maxSpeed; // Normalize speed to range [0, 1]

        // Calculate chaotic position offsets
        float shakeX = (Mathf.PerlinNoise(Time.time + chaosOffsetX, 0) - 0.5f) * chaosMultiplier * speedFactor;
        float shakeY = (Mathf.PerlinNoise(Time.time + chaosOffsetY, 1) - 0.5f) * chaosMultiplier * speedFactor;
        float shakeZ = (Mathf.PerlinNoise(Time.time + chaosOffsetZ, 2) - 0.5f) * chaosMultiplier * speedFactor;

        // Apply chaotic position offsets
        transform.localPosition += new Vector3(shakeX, shakeY, shakeZ);

        // Calculate chaotic rotation offsets
        float rotationShakeX = (Mathf.PerlinNoise(Time.time + chaosRotationOffset, 0) - 0.5f) * maxChaosAngle * speedFactor;
        float rotationShakeY = (Mathf.PerlinNoise(Time.time + chaosRotationOffset, 1) - 0.5f) * maxChaosAngle * speedFactor;
        float rotationShakeZ = (Mathf.PerlinNoise(Time.time + chaosRotationOffset, 2) - 0.5f) * maxChaosAngle * speedFactor;

        // Apply chaotic rotation offsets
        Quaternion chaosRotation = Quaternion.Euler(rotationShakeX, rotationShakeY, rotationShakeZ);
        transform.localRotation *= chaosRotation;
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
        // Choose the path based on the boolean variable
        if (intersection.choosePath2)
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
