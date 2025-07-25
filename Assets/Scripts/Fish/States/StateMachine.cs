using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public List<State> states;
    public State currentState;

    public void Initialize(EnemyAgent agent)
    {
        //Create list and serialize all fields
        states = new List<State> { agent.patrolling, agent.stalking, agent.attacking, agent.attack, agent.grabbed, agent.retreating, agent.death};
        SetStateValues(agent.patrolling, agent, agent.players);
        SetStateValues(agent.stalking, agent, agent.players);
        SetStateValues(agent.attacking, agent, agent.players);
        SetStateValues(agent.attack, agent, agent.players);
        SetStateValues(agent.grabbed, agent, agent.players);
        SetStateValues(agent.retreating, agent, agent.players);
        SetStateValues(agent.death, agent, agent.players);
        
        //Start the first state
        currentState = agent.patrolling;
        currentState.StartState();
    }
    
    public void StateTransformation(State state)
    {
        currentState.EndState();
        currentState = state;
        currentState.StartState();
    }

    private void SetStateValues(State state, EnemyAgent agent, List<Transform> players)
    {
        state.agent = agent;

        state.players = agent.players;

        state.stateMachine = this;
    }
}
