using UnityEngine;

public class TrackFollower : MonoBehaviour
{
    public Transform[] controlPoints;
    public float baseRotationSpeed = 1f;
    public float maxSegmentLength = 0.1f;

    private TrolleyController trolleyController;
    private int currentSegmentIndex = 0;
    private float t = 0f;

    void Start()
    {
        trolleyController = GetComponent<TrolleyController>();
    }

    void Update()
    {
        if (controlPoints.Length < 3)
            return;

        MoveAlongCurve();
    }

    void MoveAlongCurve()
    {
        float speed = trolleyController.CurrentSpeed;

        // Increment t based on speed and segment length
        t += (speed / 10f) * Time.deltaTime;

        // If t is greater than 1, move to the next segment
        if (t > 1f)
        {
            t = 0f;
            currentSegmentIndex += 2; // Move to the next set of points

            if (currentSegmentIndex >= controlPoints.Length - 2)
            {
                currentSegmentIndex = 0; // Loop back to the start
            }
        }

        Vector3 p0 = controlPoints[currentSegmentIndex].position;
        Vector3 p1 = controlPoints[currentSegmentIndex + 1].position;
        Vector3 p2 = controlPoints[currentSegmentIndex + 2].position;

        // Get the next position on the curve
        Vector3 nextPosition = BezierCurve.GetPoint(p0, p1, p2, t);

        // Move towards the next position
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);

        // Smoothly rotate towards the next position
        Vector3 direction = (nextPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float angle = Quaternion.Angle(transform.rotation, targetRotation);
        float rotationSpeed = baseRotationSpeed * (speed / trolleyController.maxSpeed) * (angle / 180f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
