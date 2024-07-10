using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "AttackShark", menuName = "State/Attack/AttackShark")]
public class AttackShark : Attack
{
    public float attackDistance = 1.0f;
    public Vector3 attackedOffset;
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

        Vector3 pointOfAttack = agent.transform.position + attackedOffset;

        Collider[] hits = Physics.OverlapSphere(pointOfAttack, attackDistance);
        //agent.transform.TransformDirection(Vector3.forward)
        if (hits.Any(hit => hit.CompareTag("Player")))
        {
            Collider hitSelected = hits.First(hit => hit.CompareTag("Player"));
            Vector3 direction = (hitSelected.gameObject.transform.position - pointOfAttack).normalized;
            Ray ray = new Ray(pointOfAttack, direction);

            // Debugging information
            //Debug.DrawRay(agent.transform.position, direction * attackDistance, Color.red, 2.0f);
            
            //Wall detection
            int wallLayerMask = LayerMask.GetMask("Wall");

            if (!Physics.Raycast(ray, (hitSelected.gameObject.transform.position - pointOfAttack).magnitude, wallLayerMask))
            {
                if (hitSelected.CompareTag("Player") && hitSelected.gameObject.TryGetComponent(out FirstPersonController player) && !player.isGrabbed) attackedObject = hitSelected.gameObject;
            }
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public override void Transition()
    {
        base.Transition();
        
        if(attackedObject != null) stateMachine.StateTransformation(agent.grabbed);
        else stateMachine.StateTransformation(agent.retreating);
    }
}
