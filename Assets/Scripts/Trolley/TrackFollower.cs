using UnityEngine;

public class TrackFollower : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private TrolleyController trolleyController;
    public float rotationSpeed = 5f; // Speed at which the trolley rotates

    void Start()
    {
        trolleyController = GetComponent<TrolleyController>();
    }

    void Update()
    {
        if (waypoints.Length == 0)
            return;

        MoveTowardsWaypoint();
    }

    void MoveTowardsWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        float speed = trolleyController.CurrentSpeed;

        // Move towards the waypoint
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Smoothly rotate towards the waypoint
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}