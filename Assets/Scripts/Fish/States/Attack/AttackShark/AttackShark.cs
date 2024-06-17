using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "AttackShark", menuName = "State/Attack/AttackShark")]
public class AttackShark : Attack
{
    public float attackDistance = 1.0f;
    public GameObject attackedObject;
    // ReSharper disable Unity.PerformanceAnalysis
    public override void StartState()
    {
        base.StartState();
        attackedObject = null;
    }
    
    public override void EndState()
    {
        base.EndState();
        attackedObject = null;
    }
    
    public override void MainLogic()
    {
        base.MainLogic();
        
        Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, agent.turnSpeed * Time.deltaTime);
        
        agent.transform.position += agent.velocity * Time.deltaTime;

        RaycastHit[] hit = Physics.SphereCastAll(agent.transform.position, attackDistance, 
            agent.transform.TransformDirection(Vector3.forward));

        if (hit.Any(x => x.collider.CompareTag("Player")))
        {
            RaycastHit hitSelected = hit.First(x => x.collider.CompareTag("Player"));
            attackedObject = hitSelected.collider.CompareTag("Player") ? hitSelected.collider.gameObject : null;
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public override void Transition()
    {
        base.Transition();
        
        if(attackedObject != null) stateMachine.StateTransformation(agent.grabbed);
        else stateMachine.StateTransformation(agent.stalking);
    }
}
