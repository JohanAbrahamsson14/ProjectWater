using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    public float buoyancyForce = 10.0f;
    public Vector3 windDirection = Vector3.forward;
    public float windStrength = 1f;
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.up * buoyancyForce + windDirection * windStrength, ForceMode.Acceleration);
    }
}