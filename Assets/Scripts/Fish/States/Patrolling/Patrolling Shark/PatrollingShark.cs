using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "PatrollingShark", menuName = "State/Patrolling/PatrollingShark")]
public class PatrollingShark : Patrolling
{
    private Transform selectedPlayer;
    private float patrollingTowardsPlayerStrength = 1.3f;
    public float detectionRange = 15.0f;
    public float distanceBase = 50f;
    private float currentDistance = 0f;
    public float distanceDegration = 2f;
    public float wallAvoidanceWeight = 3.0f;
    public float wallDetectionDistance = 4.0f;
    public LayerMask wallLayer;
    
    public override void StartState()
    {
        base.StartState();
        currentDistance = distanceBase;
        
        agent.speed = UnityEngine.Random.Range(agent.minSpeed, agent.maxSpeed);
        agent.turnSpeed = UnityEngine.Random.Range(agent.minTurnSpeed, agent.maxTurnSpeed);
        
        agent.speedChangeTimer = agent.speedChangeInterval;
        agent.velocity = agent.transform.forward * agent.speed;
        agent.initialDirection = agent.initialDirection.normalized;
        agent.randomMovement = new Vector3(
            UnityEngine.Random.Range(-0.3f, 0.3f),
            UnityEngine.Random.Range(-0.3f, 0.3f),
            UnityEngine.Random.Range(-0.3f, 0.3f));
    }
    
    public override void EndState()
    {
        base.EndState();
    }
    
    public override void MainLogic()
    {
        base.MainLogic();
        players.Sort((a,b) => Vector3.Distance(a.position, agent.transform.position).CompareTo(Vector3.Distance(b.position, agent.transform.position)));
        selectedPlayer = players.First();

        currentDistance -= distanceDegration * Time.deltaTime;
        currentDistance = Mathf.Clamp(currentDistance, 0, distanceBase);
        
        agent.speedChangeTimer -= Time.deltaTime;
        if (agent.speedChangeTimer <= 0)
        {
            agent.speed = UnityEngine.Random.Range(agent.minSpeed, agent.maxSpeed);
            agent.turnSpeed = UnityEngine.Random.Range(agent.minTurnSpeed, agent.maxTurnSpeed);
            agent.randomMovement = new Vector3(
                UnityEngine.Random.Range(-0.3f, 0.3f),
                UnityEngine.Random.Range(-0.3f, 0.3f),
                UnityEngine.Random.Range(-0.3f, 0.3f)
            );
            agent.speedChangeTimer = agent.speedChangeInterval;
        }
        
        agent.direction = agent.randomMovement + PatrolingTowardsTarget(selectedPlayer, patrollingTowardsPlayerStrength) + AvoidWalls(wallAvoidanceWeight);
        agent.velocity += Time.deltaTime * agent.direction;
        agent.velocity += Time.deltaTime * agent.speed * agent.velocity.normalized;
        agent.velocity = Vector3.ClampMagnitude(agent.velocity, agent.speed);

        Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, agent.turnSpeed * Time.deltaTime);

        agent.transform.position += agent.velocity * Time.deltaTime;
    }
    
    public override void Transition()
    {
        if (Vector3.Distance(agent.transform.position, selectedPlayer.position) < detectionRange) stateMachine.StateTransformation(agent.stalking);
        base.Transition();
    }
    
    private Vector3 PatrolingTowardsTarget(Transform target, float value)
    {
        Vector3 patrollingPoint = target.position + Random.onUnitSphere * currentDistance;
        //The Vector from the agent to the selected player
        Vector3 towardsVector = patrollingPoint - agent.transform.position;
        
        return towardsVector.normalized * value;
    }
    
    Vector3 AvoidWalls(float value)
    {
        Vector3 avoidanceForce = Vector3.zero;
        RaycastHit hit;

        // Cast rays in multiple directions to detect walls
        Vector3[] rayDirections = {
            agent.transform.forward,
            (agent.transform.forward + agent.transform.right).normalized,
            (agent.transform.forward - agent.transform.right).normalized,
            (agent.transform.forward + agent.transform.up).normalized,
            (agent.transform.forward - agent.transform.up).normalized
        };

        foreach (Vector3 dir in rayDirections)
        {
            if (Physics.Raycast(agent.transform.position, dir, out hit, wallDetectionDistance, wallLayer))
            {
                avoidanceForce += (agent.transform.position - hit.point).normalized;
            }
        }

        // Normalize and return the avoidance force
        return avoidanceForce.normalized*value;
    }
}
