using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "PatrollingShark", menuName = "State/Patrolling/PatrollingShark")]
public class PatrollingShark : Patrolling
{
    public float detectionRange = 10.0f;
    
    public override void StartState()
    {
        base.StartState();
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
        
        agent.direction = agent.randomMovement;
        agent.velocity += Time.deltaTime * agent.direction;
        agent.velocity += Time.deltaTime * agent.speed * agent.velocity.normalized;
        agent.velocity = Vector3.ClampMagnitude(agent.velocity, agent.speed);

        Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, agent.turnSpeed * Time.deltaTime);

        agent.transform.position += agent.velocity * Time.deltaTime;
    }
    
    public override void Transition()
    {
        if (Vector3.Distance(agent.transform.position, players.First().position) < detectionRange) stateMachine.StateTransformation(agent.stalking);
        base.Transition();
    }

    public void OnDrawGizmos()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(agent.transform.position, detectionRange);
    }
}
