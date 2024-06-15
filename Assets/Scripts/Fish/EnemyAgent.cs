using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgent : MonoBehaviour
{
    public enum States {Patrolling, Stalking, Attacking, Attack, Grabbed, Retreating, Death}
    
    protected StateMachine stateMachine;
    public Patrolling patrolling;
    public Stalking stalking;
    public Attacking attacking;
    public Attack attack;
    public Grabbed grabbed;
    public Retreating retreating;
    public Death death;
    
    public States currentState;
    public List<Transform> players;
    public float health = 100.0f;
    
    public float minSpeed = 3.3f;
    public float maxSpeed = 3.7f;
    public float minTurnSpeed = 4.5f;
    public float maxTurnSpeed = 5.5f;
    public float wallAvoidanceWeight = 5.0f;
    public float wallDetectionDistance = 2.0f;
    public LayerMask wallLayer;
    public Vector3 initialDirection = Vector3.forward;
    [NonSerialized] public Vector3 velocity;
    [NonSerialized] public Vector3 direction;
    public float speedChangeInterval = 5.0f; // Interval in seconds for changing speed
    public float speedChangeTimer;
    [NonSerialized] public Vector3 randomMovement;
    [NonSerialized] public float speed = 3.5f;
    [NonSerialized] public float turnSpeed = 5.0f;

    public void Awake()
    {
        stateMachine = new StateMachine();
    }
}
