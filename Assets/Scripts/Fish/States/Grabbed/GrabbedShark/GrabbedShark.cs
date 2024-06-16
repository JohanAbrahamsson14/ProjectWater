using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;


[CreateAssetMenu(fileName = "GrabbedShark", menuName = "State/Grabbed/GrabbedShark")]
public class GrabbedShark : Grabbed
{
    
    public float timeToRealse = 5.0f;
    public float currentTime;
    public GameObject grabbedObject;
    public float attackDistance;
    
    public float selectionDistance = 30;
    public Vector3 selectionPos;
    public float selectionStrength = 3;
    public float grabbOffsetForward = 0.5f;
    
    public override void StartState()
    {
        base.StartState();

        currentTime = 0;

        selectionPos = UnityEngine.Random.onUnitSphere * selectionDistance;
        
        //reevaluate the value so that we have the same thing
        /*
        if (Physics.SphereCast(agent.transform.position, attackDistance, agent.transform.TransformDirection(Vector3.forward), out var hit))
        {
            grabbedObject = hit.collider.CompareTag("Player") ? hit.collider.gameObject : null;
        }
        */
        
        RaycastHit[] hit = Physics.RaycastAll(agent.transform.position,
            agent.transform.TransformDirection(Vector3.forward), attackDistance);

        if (hit.Any(x => x.collider.CompareTag("Player")))
        {
            RaycastHit hitSelected = hit.First(x => x.collider.CompareTag("Player"));
            grabbedObject = hitSelected.collider.CompareTag("Player") ? hitSelected.collider.gameObject : null;
        }

        if (grabbedObject == null) return;
        grabbedObject.transform.SetParent(agent.transform);
        grabbedObject.transform.localPosition = Vector3.forward*grabbOffsetForward;
        grabbedObject.transform.Rotate(Vector3.forward, 90);
        grabbedObject.GetComponent<FirstPersonController>().SetGrabbed(true);
    }
    
    public override void EndState()
    {
        base.EndState();
        
        currentTime = 0;
        
        if (grabbedObject == null) return;
        grabbedObject.transform.rotation = quaternion.identity;
        grabbedObject.transform.SetParent(null);
        grabbedObject.GetComponent<FirstPersonController>().SetGrabbed(false);
    }
    
    public override void MainLogic()
    {
        base.MainLogic();
        
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
        
        agent.direction = agent.randomMovement + AttackingMovementTarget(selectionPos, selectionStrength);
        agent.velocity += Time.deltaTime * agent.direction;
        agent.velocity += Time.deltaTime * agent.speed * agent.velocity.normalized;
        agent.velocity = Vector3.ClampMagnitude(agent.velocity, agent.speed);

        Quaternion targetRotation = Quaternion.LookRotation(agent.velocity.normalized);
        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, agent.turnSpeed * Time.deltaTime);

        agent.transform.position += agent.velocity * Time.deltaTime;
    }
    
    public override void Transition()
    {
        if (currentTime >= timeToRealse) stateMachine.StateTransformation(agent.retreating);
        base.Transition();
    }
    
    private Vector3 AttackingMovementTarget(Vector3 pos, float value)
    {
        //The Vector from the agent to the selected player
        Vector3 towardsVector = pos - agent.transform.position;
        
        return towardsVector.normalized * value;
    }
}
