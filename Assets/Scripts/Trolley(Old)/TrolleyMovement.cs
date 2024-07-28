using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class TrolleyMovement : MonoBehaviour
{
    public float speed;
    public float turningSpeed;
    private Rigidbody rb; 
    [SerializeField] private Sensor sensor;
    private GameObject nextDestination;
   

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        
        if (sensor.detectedDestinations.Count == 0) return;
        Vector3 lookPos = sensor.detectedDestinations.First().transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turningSpeed);
        
        rb.velocity = transform.forward * speed;
        /*
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, sensor.detectedDestinations.First().transform.position, step);
        */
    }
}
