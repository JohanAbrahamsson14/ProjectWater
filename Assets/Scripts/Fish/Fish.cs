using System;
using UnityEngine;
using System.Collections.Generic;

public class Fish : MonoBehaviour
{
    public float speed = 2.0f;
    public float turnSpeed = 5.0f;
    public float neighborDistance = 3.0f;
    public float separationDistance = 1.0f;
    public float cohesionWeight = 1.0f;
    public float alignmentWeight = 1.0f;
    public float separationWeight = 1.5f;
    public float predatorAvoidanceWeight = 3.0f;
    public float predatorDetectionRadius = 5.0f;
    public float wallAvoidanceWeight = 5.0f;
    public float wallDetectionDistance = 2.0f;
    public LayerMask wallLayer;
    
    public Collider collider;

    public Vector3 initialDirection = Vector3.forward;

    public List<Fish> neighborFish;
    private Vector3 velocity;

    private Vector3 direction;
    
    void Start()
    {
        velocity = transform.forward * speed;
        initialDirection = initialDirection.normalized;
    }

    void Update()
    {
        neighborFish = GetNeighbors();
        
        Vector3 cohesion = Cohesion() * cohesionWeight;
        Vector3 alignment = Alignment() * alignmentWeight;
        Vector3 separation = Separation() * separationWeight;
        Vector3 predatorAvoidance = AvoidPredators() * predatorAvoidanceWeight;
        Vector3 wallAvoidance = AvoidWalls() * wallAvoidanceWeight;
        
        direction = cohesion + alignment + separation + predatorAvoidance + wallAvoidance;
        velocity += Time.deltaTime * direction;
        velocity += Time.deltaTime * speed * velocity.normalized;
        velocity = Vector3.ClampMagnitude(velocity, speed);

        Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        transform.position += velocity * Time.deltaTime;
    }

    Vector3 Cohesion()
    {
        Vector3 centerOfMass = Vector3.zero;

        foreach (Fish fish in neighborFish)
        {
            centerOfMass += fish.transform.position;
        }

        if (neighborFish.Count == 0) return Vector3.zero;

        centerOfMass /= neighborFish.Count;
        return (centerOfMass - transform.position).normalized;
    }

    Vector3 Alignment()
    {
        Vector3 averageHeading = Vector3.zero;

        foreach (Fish fish in neighborFish)
        {
            averageHeading += fish.velocity;
        }

        if (neighborFish.Count == 0) return Vector3.zero;

        averageHeading /= neighborFish.Count;
        return averageHeading.normalized;
    }

    Vector3 Separation()
    {
        Vector3 separationForce = Vector3.zero;

        foreach (Fish fish in neighborFish)
        {
            if (Vector3.Distance(transform.position, fish.transform.position) < separationDistance)
            {
                separationForce += transform.position - fish.transform.position;
            }
        }

        return separationForce.normalized;
    }

    Vector3 AvoidPredators()
    {
        Collider[] predators = Physics.OverlapSphere(transform.position, predatorDetectionRadius, LayerMask.GetMask("Predator"));
        Vector3 avoidanceForce = Vector3.zero;

        foreach (Collider predator in predators)
        {
            avoidanceForce += transform.position - predator.transform.position;
        }

        return avoidanceForce.normalized;
    }
    
    Vector3 AvoidWalls()
    {
        Vector3 avoidanceForce = Vector3.zero;
        RaycastHit hit;

        // Cast rays in multiple directions to detect walls
        Vector3[] rayDirections = {
            transform.forward,
            (transform.forward + transform.right).normalized,
            (transform.forward - transform.right).normalized,
            (transform.forward + transform.up).normalized,
            (transform.forward - transform.up).normalized
        };

        foreach (Vector3 dir in rayDirections)
        {
            if (Physics.Raycast(transform.position, dir, out hit, wallDetectionDistance, wallLayer))
            {
                avoidanceForce += (transform.position - hit.point).normalized;
            }
        }

        // Normalize and return the avoidance force
        return avoidanceForce.normalized;
    }

    List<Fish> GetNeighbors()
    {
        List<Fish> neighbors = new List<Fish>();
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, neighborDistance, LayerMask.GetMask("Fish"));

        foreach (Collider obj in nearbyObjects)
        {
            if (obj != collider)
            {
                obj.gameObject.TryGetComponent<Fish>(out Fish fish);
                neighbors.Add(fish);
            }
        }

        return neighbors;
    }

    public void OnDrawGizmos()
    {
        Vector3[] rayDirections = {
            transform.forward,
            (transform.forward + transform.right).normalized,
            (transform.forward - transform.right).normalized,
            (transform.forward + transform.up).normalized,
            (transform.forward - transform.up).normalized
        };
        
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position, neighborDistance);
        Gizmos.color=Color.green;
        Gizmos.DrawRay(transform.position, direction);
        Gizmos.color=Color.yellow;
        Gizmos.DrawRay(transform.position, velocity);
        
        Gizmos.color=Color.cyan;
        Gizmos.DrawRay(transform.position, Cohesion() * cohesionWeight);
        Gizmos.color=Color.magenta;
        Gizmos.DrawRay(transform.position, Alignment() * alignmentWeight);
        Gizmos.color=Color.black;
        Gizmos.DrawRay(transform.position, Separation() * separationWeight);

        Gizmos.color = Color.blue;
        foreach (var ray in rayDirections)
        {
            Gizmos.DrawRay(transform.position,ray*wallDetectionDistance);
        }
    }
}
