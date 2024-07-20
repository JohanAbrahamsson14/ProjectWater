using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class Fish : MonoBehaviour
{
    [Header("Speed")]
    public float minSpeed = 3.3f;
    public float maxSpeed = 3.7f;
    public float speedChangeInterval = 5.0f; // Interval in seconds for changing speed
    [Header("TurnSpeed")]
    public float minTurnSpeed = 4.5f;
    public float maxTurnSpeed = 5.5f;
    [Header("Weights")]
    public float cohesionWeight = 1.0f;
    public float alignmentWeight = 1.0f;
    public float separationWeight = 1.5f;
    public float wallAvoidanceWeight = 5.0f;
    public float selectionPointWeight = 10f;
    [Header("Neighbours")]
    public float neighborDistance = 3.0f;
    public List<Fish> neighborFish;
    [Header("Seperation & Predetor")]
    public float separationDistance = 1.0f;
    public float predatorAvoidanceWeight = 3.0f;
    public float predatorDetectionRadius = 5.0f;
    [Header("Wall")]
    public float wallDetectionDistance = 2.0f;
    public LayerMask wallLayer;
    [Header("Selection Point")]
    public float distanceFromSelectionPoint = 10f;
    public Vector3 selectionPoint;
    [Header("Other")]
    public Collider collider = null;
    
    private Vector3 velocity;

    private Vector3 direction;
    
    private float speedChangeTimer;
    private Vector3 randomMovement;
    
    private float speed = 3.5f;
    private float turnSpeed = 5.0f;
    
    private Renderer renderer;
    
    void Start()
    {
        renderer = GetComponent<Renderer>();
        renderer.material.SetFloat("_RandomValue", UnityEngine.Random.Range(0,99999));
        
        speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
        turnSpeed = UnityEngine.Random.Range(minTurnSpeed, maxTurnSpeed);
        
        speedChangeTimer = speedChangeInterval;
        velocity = transform.forward * speed;
        randomMovement = new Vector3(
            UnityEngine.Random.Range(-0.3f, 0.3f),
            UnityEngine.Random.Range(-0.3f, 0.3f),
            UnityEngine.Random.Range(-0.3f, 0.3f)
            );
    }

    void Update()
    {
        neighborFish = GetNeighbors();
        
        Vector3 cohesion = Cohesion() * cohesionWeight;
        Vector3 alignment = Alignment() * alignmentWeight;
        Vector3 separation = Separation() * separationWeight;
        Vector3 predatorAvoidance = AvoidPredators() * predatorAvoidanceWeight;
        Vector3 wallAvoidance = AvoidWalls() * wallAvoidanceWeight;
        Vector3 selectionPoint = SelectionPoint() * selectionPointWeight;
        
        speedChangeTimer -= Time.deltaTime;
        if (speedChangeTimer <= 0)
        {
            speed = UnityEngine.Random.Range(minSpeed, maxSpeed);
            turnSpeed = UnityEngine.Random.Range(minTurnSpeed, maxTurnSpeed);
            randomMovement = new Vector3(
                UnityEngine.Random.Range(-0.3f, 0.3f),
                UnityEngine.Random.Range(-0.3f, 0.3f),
                UnityEngine.Random.Range(-0.3f, 0.3f)
            );
            speedChangeTimer = speedChangeInterval;
        }
        
        direction = cohesion + alignment + separation + predatorAvoidance + wallAvoidance + selectionPoint + randomMovement;
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
        Vector3 position = transform.position;

        foreach (Fish fish in neighborFish)
        {
            if (Vector3.Distance(position, fish.transform.position) < separationDistance)
            {
                separationForce += position - fish.transform.position;
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
    
    Vector3 SelectionPoint()
    {
        Vector3 returnToStart;
        if(Vector3.Distance(transform.position, selectionPoint)>distanceFromSelectionPoint) return (selectionPoint-transform.position).normalized;
        return Vector3.zero;
    }

    List<Fish> GetNeighbors()
    {
        List<Fish> neighbors = new List<Fish>();
        
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, neighborDistance, LayerMask.GetMask("Fish"));
        

        /*
        RaycastHit hit;
        Vector3[] rayDirectionsNeighbours = {
            transform.forward,
            (transform.forward + transform.right).normalized,
            (transform.forward - transform.right).normalized,
            (transform.forward + transform.up).normalized,
            (transform.forward - transform.up).normalized,
            
            (transform.forward + transform.right + transform.up).normalized,
            (transform.forward - transform.right + transform.up).normalized,
            (transform.forward + transform.right - transform.up).normalized,
            (transform.forward - transform.right - transform.up).normalized,
            
            (transform.forward*2 + transform.right).normalized,
            (transform.forward*2 - transform.right).normalized,
            (transform.forward*2 + transform.up).normalized,
            (transform.forward*2 - transform.up).normalized,
            
            (transform.forward*2 + transform.right + transform.up).normalized,
            (transform.forward*2 - transform.right + transform.up).normalized,
            (transform.forward*2 + transform.right - transform.up).normalized,
            (transform.forward*2 - transform.right - transform.up).normalized,
            
            transform.right,
            -transform.right,
            transform.up,
            -transform.up,
            (-transform.forward + transform.right).normalized,
            (-transform.forward - transform.right).normalized,
            (-transform.forward + transform.up).normalized,
            (-transform.forward - transform.up).normalized
            
        };
        */
        /*
        foreach (Vector3 dir in rayDirectionsNeighbours)
        {
            if (Physics.Raycast(transform.position, dir, out hit, neighborDistance, LayerMask.GetMask("Fish")))
            {
                if (hit.collider != collider)
                {
                    hit.collider.gameObject.TryGetComponent<Fish>(out Fish fish);
                    if(!neighbors.Contains(fish)) neighbors.Add(fish);
                }
            }
        }
        */
        
        
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
        
        Gizmos.color=Color.white;
        Gizmos.DrawWireSphere(selectionPoint, distanceFromSelectionPoint);
    }
}
