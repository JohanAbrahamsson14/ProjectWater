using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public List<Transform> detectedDestinations;
    public Transform train;

    private void Awake()
    {
        detectedDestinations = new List<Transform>();
    }

    private void Update()
    {
        
        for (int i = 1; i < detectedDestinations.Count; i++)
        { 
            LerpFunc(detectedDestinations[i-1], detectedDestinations[i]);
        }
        
    }

    private Vector3 LerpFunc(Transform a, Transform b)
    {
        return Vector3.Lerp(a.position, b.position, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Destination")
        {
            detectedDestinations.Add(other.gameObject.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Destination")
        {
            detectedDestinations.Remove(other.gameObject.transform);
        }
    }
}
