using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = "AttackingShark", menuName = "State/Attacking/AttackingShark")]
public class AttackingShark : Attacking
{
    private Transform selectedPlayer;
    public float attackRange = 6.0f;
    
    public override void StartState()
    {
        base.StartState();
        
        selectedPlayer = agent.players.First();
    }
    
    public override void EndState()
    {
        base.EndState();
    }
    
    public override void MainLogic()
    {
        Debug.Log("Attacking");
        base.MainLogic();
    }
    
    public override void Transition()
    {
        if (Vector3.Distance(agent.transform.position, selectedPlayer.position) < attackRange) stateMachine.StateTransformation(agent.attack);
        base.Transition();
    }
}
