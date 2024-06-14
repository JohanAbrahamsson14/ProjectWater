using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public List<State> states;
    public State currentState;

    public void Initialize()
    {
        states = new List<State>();
    }
    
    public void StateTransformation(State state)
    {
        currentState.EndState();
        currentState = state;
        currentState.StartState();
    }
}
