using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "AttackShark", menuName = "State/Attack/AttackShark")]
public class AttackShark : Attack
{
    public float attackDistance = 1.0f;
    
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
        Debug.Log("Attack");
        base.MainLogic();
    }
    
    public override void Transition()
    {
        stateMachine.StateTransformation(agent.grabbed);
        base.Transition();
    }
}
