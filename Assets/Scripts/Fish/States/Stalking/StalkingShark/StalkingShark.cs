using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "StalkingShark", menuName = "State/Stalking/StalkingShark")]
public class StalkingShark : Stalking
{
    private Transform selectedPlayer;
    public float stalkingStrength = 2.0f;
    public float minimumRange = 8.0f;
    public float maximumDistance = 20.0f;
    public float timeToAttack = 15.0f;
    private float currentTime;
    public bool isMovingAway;
    
    public override void StartState()
    {
        base.StartState();

        currentTime = 0;
        
        selectedPlayer = agent.players.First();
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
        
        currentTime = 0;
    }
    
    public override void MainLogic()
    {
        base.MainLogic();
        players.Sort((a,b) => Vector3.Distance(a.position, agent.transform.position).CompareTo(Vector3.Distance(b.position, agent.transform.position)));
        selectedPlayer = players.First();
        
        currentTime += Time.deltaTime;
        
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
        
        agent.direction = agent.randomMovement + StalkingMovementTarget(selectedPlayer, stalkingStrength);
        agent.velocity += Time.deltaTime * agent.direction;
        agent.velocity += Time.deltaTime * agent.speed * agent.velocity.normalized;
        agent.velocity = Vector3.ClampMagnitude(agent.velocity, agent.speed);

        Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, agent.turnSpeed * Time.deltaTime);

        agent.transform.position += agent.velocity * Time.deltaTime;
    }
    
    public override void Transition()
    {
        
        Vector3 dirFromAtoB = (selectedPlayer.position - agent.transform.position).normalized;
        float dotProd = Vector3.Dot(dirFromAtoB, agent.transform.forward);
        
        if (currentTime >= timeToAttack && dotProd > 0.95) stateMachine.StateTransformation(agent.attacking);
        base.Transition();
    }

    private Vector3 StalkingMovementTarget(Transform target, float value)
    {
        //The Vector from the agent to the selected player
        Vector3 towardsVector = target.position - agent.transform.position;

        //To tell the system if 
        if (towardsVector.magnitude < minimumRange && !isMovingAway) isMovingAway = true;
        if (towardsVector.magnitude > maximumDistance && isMovingAway) isMovingAway = false;

        towardsVector = isMovingAway ? -towardsVector : towardsVector;
        
        return towardsVector.normalized * value;
    }
}
