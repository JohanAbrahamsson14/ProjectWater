using UnityEngine;

public class TrackFollower : MonoBehaviour
{
    public Transform[] keyPoints; // Points the trolley should touch
    public Transform[] controlPoints; // Points defining the curvature

    public float baseRotationSpeed = 1f;
    public float maxTiltAngle = 30f; // Increase maximum tilt angle for more noticeable effect
    public float tiltDamping = 5f; // Damping factor for smoothing the tilt
    private TrolleyController trolleyController;
    private int currentSegmentIndex = 0;
    private float traveledDistance = 0f;
    private float segmentLength = 0f;
    private float currentTiltAngle = 0f; // Current tilt angle for smoothing

    void Start()
    {
        if (keyPoints.Length != controlPoints.Length + 1)
        {
            Debug.LogError("There should be one more key point than control points.");
            return;
        }

        trolleyController = GetComponent<TrolleyController>();
        CalculateSegmentLength();
    }

    void Update()
    {
        if (keyPoints.Length < 2 || controlPoints.Length < 1)
            return;

        MoveAlongCurve();
    }

    void MoveAlongCurve()
    {
        float speed = trolleyController.CurrentSpeed;
        traveledDistance += speed * Time.deltaTime;

        // Calculate t based on traveled distance and segment length
        float t = traveledDistance / segmentLength;

        // If t is greater than 1, move to the next segment
        if (t > 1f)
        {
            traveledDistance -= segmentLength; // Reset traveled distance for the next segment
            currentSegmentIndex += 1;
            if (currentSegmentIndex >= keyPoints.Length - 1)
            {
                currentSegmentIndex = 0; // Loop back to the start
            }
            CalculateSegmentLength();
            t = traveledDistance / segmentLength; // Recalculate t for the new segment
        }

        Vector3 p0 = keyPoints[currentSegmentIndex].position;
        Vector3 p1 = controlPoints[currentSegmentIndex].position;
        Vector3 p2 = keyPoints[currentSegmentIndex + 1].position;

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
        Vector3 p0 = keyPoints[currentSegmentIndex].position;
        Vector3 p1 = controlPoints[currentSegmentIndex].position;
        Vector3 p2 = keyPoints[currentSegmentIndex + 1].position;
        segmentLength = BezierCurve.GetApproximateLength(p0, p1, p2);
    }
}
