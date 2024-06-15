using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "DeathShark", menuName = "State/Death/DeathShark")]
public class DeathShark : Death
{
    public float detectionRange = 10.0f;
    
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
        Debug.Log("Death");
        base.MainLogic();
    }
    
    public override void Transition()
    {
        if (Vector3.Distance(agent.transform.position, players.First().position) < detectionRange) stateMachine.StateTransformation(agent.stalking);
        base.Transition();
    }
}
