using System;
using UnityEngine;

public class Shark : MonoBehaviour
{
    public enum State { Patrolling, Stalking, Attacking, Retreating }
    
    public StateMachine stateMachine;
    public Patrolling partrolling;
    
    public State currentState;
    public Transform player;
    public float detectionRange = 10.0f;
    public float attackRange = 2.0f;
    public float health = 100.0f;
    public float retreatThreshold = 20.0f;

    public void Start()
    {
        stateMachine.Initialize();
    }

    void Update()
    {
        stateMachine.currentState.MainLogic();
        
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                if (Vector3.Distance(transform.position, player.position) < detectionRange)
                    currentState = State.Stalking;
                break;
            case State.Stalking:
                Stalk();
                if (Vector3.Distance(transform.position, player.position) < attackRange)
                    currentState = State.Attacking;
                break;
            case State.Attacking:
                Attack();
                if (health < retreatThreshold)
                    currentState = State.Retreating;
                break;
            case State.Retreating:
                Retreat();
                if (health > retreatThreshold)
                    currentState = State.Patrolling;
                break;
        }
    }

    void Patrol()
    {
        // Patrol behavior
    }

    void Stalk()
    {
        // Stalking behavior
    }

    void Attack()
    {
        // Attacking behavior
    }

    void Retreat()
    {
        // Retreating behavior
    }
}

