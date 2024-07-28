using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonMovement : MonoBehaviour
{
    public float speed;
    public float turningSpeed;
    private Rigidbody rb; 
    public GameObject toFollowJoint;
    //public GameObject connectionJoint;
   

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 lookPos = toFollowJoint.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turningSpeed);
        
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, toFollowJoint.transform.position, step);

        /*
        if (Vector3.Distance(transform.position, toFollow.transform.position) < 0.001f)
        {
            toFollow.transform.position *= -1.0;
        }
        */
    }
}
