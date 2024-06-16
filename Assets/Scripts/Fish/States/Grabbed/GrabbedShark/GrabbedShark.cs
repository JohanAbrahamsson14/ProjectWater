using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
[CreateAssetMenu(fileName = "GrabbedShark", menuName = "State/Grabbed/GrabbedShark")]
public class GrabbedShark : Grabbed
{
    
    public float timeToRealse = 5.0f;
    public float currentTime;
    public GameObject grabbedObject;
    public float attackDistance;
    
    public override void StartState()
    {
        base.StartState();

        currentTime = 0;
        
        //reevaluate the value so that we have the same thing
        if (Physics.Raycast(agent.transform.position, agent.transform.TransformDirection(Vector3.forward), out var hit, attackDistance))
        {
            grabbedObject = hit.collider.CompareTag("Player") ? hit.collider.gameObject : null;
        }

        if (grabbedObject == null) return;
        grabbedObject.transform.SetParent(agent.transform);
        grabbedObject.transform.localPosition = Vector3.zero;
        grabbedObject.transform.Rotate(Vector3.forward, 90);
    }
    
    public override void EndState()
    {
        base.EndState();
        
        currentTime = 0;
        
        if (grabbedObject == null) return;
        grabbedObject.transform.rotation = quaternion.identity;
        grabbedObject.transform.SetParent(null);
    }
    
    public override void MainLogic()
    {
        Debug.Log("Grabbed");
        base.MainLogic();

        currentTime += Time.deltaTime;
    }
    
    public override void Transition()
    {
        if (currentTime >= timeToRealse) stateMachine.StateTransformation(agent.retreating);
        base.Transition();
    }
}
