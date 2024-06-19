using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "AttackingShark", menuName = "State/Attacking/AttackingShark")]
public class AttackingShark : Attacking
{
    private Transform selectedPlayer;
    public float attackRange = 6.0f;
    public float attackingStrength = 4.0f;
    public float speedIncreaseMultiplyer = 2.0f;
    
    public override void StartState()
    {
        base.StartState();
        
        players.Sort((a,b) => Vector3.Distance(a.position, agent.transform.position).CompareTo(Vector3.Distance(b.position, agent.transform.position)));
        selectedPlayer = players.First();
        agent.speed = UnityEngine.Random.Range(agent.minSpeed, agent.maxSpeed);
        agent.turnSpeed = UnityEngine.Random.Range(agent.minTurnSpeed, agent.maxTurnSpeed);
        
        agent.speedChangeTimer = agent.speedChangeInterval;
        agent.velocity = agent.transform.forward * agent.speed;
        agent.initialDirection = agent.initialDirection.normalized;
        agent.randomMovement = new Vector3(
            UnityEngine.Random.Range(-0.3f, 0.3f),
            UnityEngine.Random.Range(-0.3f, 0.3f),
            UnityEngine.Random.Range(-0.3f, 0.3f));

        agent.speed *= speedIncreaseMultiplyer;
        agent.turnSpeed /= speedIncreaseMultiplyer;
    }
    
    public override void EndState()
    {
        base.EndState();
        
        agent.speed /= speedIncreaseMultiplyer;
        agent.turnSpeed *= speedIncreaseMultiplyer;
    }
    
    public override void MainLogic()
    {
        base.MainLogic();
        
        players.Sort((a,b) => Vector3.Distance(a.position, agent.transform.position).CompareTo(Vector3.Distance(b.position, agent.transform.position)));
        selectedPlayer = players.First();
        
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
        
        agent.direction = agent.randomMovement + AttackingMovementTarget(selectedPlayer, attackingStrength);
        agent.velocity += Time.deltaTime * agent.direction;
        agent.velocity += Time.deltaTime * agent.speed * agent.velocity.normalized;
        agent.velocity = Vector3.ClampMagnitude(agent.velocity, agent.speed);

        Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, agent.turnSpeed * Time.deltaTime);

        agent.transform.position += agent.velocity * Time.deltaTime;
    }
    
    public override void Transition()
    {
        base.Transition();
        if (Vector3.Distance(agent.transform.position, selectedPlayer.position) < attackRange) stateMachine.StateTransformation(agent.attack);
        Vector3 dirFromAtoB = (selectedPlayer.position - agent.transform.position).normalized;
        float dotProd = Vector3.Dot(dirFromAtoB, agent.transform.forward);
        if(dotProd < 0.35f) stateMachine.StateTransformation(agent.retreating);
    }
    
    private Vector3 AttackingMovementTarget(Transform target, float value)
    {
        //The Vector from the agent to the selected player
        Vector3 towardsVector = target.position - agent.transform.position;
        
        return towardsVector.normalized * value;
    }
}
