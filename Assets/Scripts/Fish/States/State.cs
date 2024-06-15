using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : ScriptableObject
{
    public List<Transform> players;
    public EnemyAgent agent;
    public StateMachine stateMachine;
    
    //Initialize
    public virtual void Initialize() { }
    //Start
    public virtual void StartState() { }
    //End
    public virtual void EndState() { }
    //Main Logic
    public virtual void MainLogic() { }
    //Transition
    public virtual void Transition() { }
}
