using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "RetreatingShark", menuName = "State/Retreating/RetreatingShark")]
public class RetreatingShark : Retreating
{
    public float timeToBecomeNormal = 5.0f;
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
        Debug.Log("Retreating");
        base.MainLogic();
        currentTime += Time.deltaTime;
    }
    
    public override void Transition()
    {
        if (currentTime >= timeToBecomeNormal) stateMachine.StateTransformation(agent.patrolling);
        base.Transition();
    }
}
