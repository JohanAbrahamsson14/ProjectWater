using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "RetreatingShark", menuName = "State/Retreating/RetreatingShark")]
public class RetreatingShark : Retreating
{
    public float timeToBecomeNormal = 5.0f;
    public float currentTime;
    
    
    private Transform selectedPlayer;
    public float selectionDistance = 30;
    public Vector3 selectionPos;
    public float selectionPlayerStrength = 5;
    public float selectionStrength = 3;
    public float speedIncreaseMultiplyer = 2.0f;
    
    public override void StartState()
    {
        base.StartState();

        players.Sort((a,b) => Vector3.Distance(a.position, agent.transform.position).CompareTo(Vector3.Distance(a.position, agent.transform.position)));
        selectedPlayer = players.First();
        
        selectionPos = UnityEngine.Random.onUnitSphere * selectionDistance;
        
        currentTime = 0;
        
        agent.speed *= speedIncreaseMultiplyer;
    }
    
    public override void EndState()
    {
        base.EndState();
        
        currentTime = 0;
        
        agent.speed /= speedIncreaseMultiplyer;
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
            agent.randomMovement = new Vector3(
                UnityEngine.Random.Range(-0.3f, 0.3f),
                UnityEngine.Random.Range(-0.3f, 0.3f),
                UnityEngine.Random.Range(-0.3f, 0.3f)
            );
            agent.speedChangeTimer = agent.speedChangeInterval;
        }
        
        agent.direction = agent.randomMovement + RandomPointMovement(selectionPos, selectionStrength) + AwayFromPlayer(selectedPlayer, selectionPlayerStrength);
        agent.velocity += Time.deltaTime * agent.direction;
        agent.velocity += Time.deltaTime * agent.speed * agent.velocity.normalized;
        agent.velocity = Vector3.ClampMagnitude(agent.velocity, agent.speed);

        Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, agent.turnSpeed * Time.deltaTime);

        agent.transform.position += agent.velocity * Time.deltaTime;
    }
    
    public override void Transition()
    {
        if (currentTime >= timeToBecomeNormal) stateMachine.StateTransformation(agent.patrolling);
        base.Transition();
    }
    
    private Vector3 RandomPointMovement(Vector3 target, float value)
    {
        //The Vector from the agent to the selected player
        Vector3 towardsVector = agent.transform.position - target;
        
        return towardsVector.normalized * value;
    }
    
    private Vector3 AwayFromPlayer(Transform target, float value)
    {
        //The Vector from the selected player to the agent
        Vector3 towardsVector = target.position - agent.transform.position;
        
        return towardsVector.normalized * value;
    }
}
