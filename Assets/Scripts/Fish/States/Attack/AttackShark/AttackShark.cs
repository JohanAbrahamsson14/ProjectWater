using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "AttackShark", menuName = "State/Attack/AttackShark")]
public class AttackShark : Attack
{
    public float attackDistance = 1.0f;
    public GameObject attackedObject;
    public override void StartState()
    {
        base.StartState();
    }
    
    public override void EndState()
    {
        base.EndState();
    }
    
    public override void MainLogic()
    {
        base.MainLogic();
        
        Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, agent.turnSpeed * Time.deltaTime);
        
        agent.transform.position += agent.velocity * Time.deltaTime;

        if (Physics.Raycast(agent.transform.position, agent.transform.TransformDirection(Vector3.forward), out var hit, attackDistance))
        {
            attackedObject = hit.collider.CompareTag("Player") ? hit.collider.gameObject : null;
        }
    }
    
    public override void Transition()
    {
        base.Transition();
        
        if(attackedObject != null) stateMachine.StateTransformation(agent.grabbed);
        else stateMachine.StateTransformation(agent.stalking);
    }
}
