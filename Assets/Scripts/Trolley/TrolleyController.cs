using UnityEngine;

public class TrolleyController : MonoBehaviour
{
    public float acceleration = 2f;
    public float brakeForce = 5f;
    public float maxSpeed = 20f;
    public AudioSource hornSound;

    private float currentSpeed = 0f;
    private Rigidbody rb;

    public float CurrentSpeed => currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            currentSpeed -= brakeForce * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            hornSound.Play();
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);
    }
}