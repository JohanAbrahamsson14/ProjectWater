using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shark : EnemyAgent, IDamageAble
{
    public float retreatThreshold = 20.0f;
    public float maxHealth { get; set; }
    public float currentHealth { get; set; }
    public void Start()
    {
        stateMachine.Initialize(this);
        maxHealth = currentHealth;
    }

    void Update()
    {
        stateMachine.currentState.MainLogic();
        stateMachine.currentState.Transition();
        /*
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
        */
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
    public void GetDamaged(int value)
    {
        currentHealth -= value;
        if (currentHealth <= 0) Death();
    }

    public void Death()
    {
        stateMachine.StateTransformation(death);
    }
}

