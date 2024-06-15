using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "GrabbedShark", menuName = "State/Grabbed/GrabbedShark")]
public class GrabbedShark : Grabbed
{
    public float timeToRealse = 5.0f;
    public float currentTime;
    
    public override void StartState()
    {
        base.StartState();

        currentTime = 0;
    }
    
    public override void EndState()
    {
        base.EndState();
        
        currentTime = 0;
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
