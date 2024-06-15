using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "StalkingShark", menuName = "State/Stalking/StalkingShark")]
public class StalkingShark : Stalking
{
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
        Debug.Log("stalking now");
        base.MainLogic();
    }
    
    public override void Transition()
    {
        base.Transition();
    }
}
