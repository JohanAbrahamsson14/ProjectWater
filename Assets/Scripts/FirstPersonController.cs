using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    public float swimSpeed = 3.0f;
    public float swimJumpSpeed = 4.0f;
    public float swimGravity = 2.0f;  // Adjusted for a more realistic sinking effect
    public float passiveSinkSpeed = 1.0f;  // Speed at which the player sinks passively
    public float waterSurfaceLevel = 5.0f;  // Example water surface level, adjust as needed

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private bool isInWater = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (isInWater)
        {
            WaterMovement();
        }
        else
        {
            GroundMovement();
        }
    }

    void GroundMovement()
    {
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void WaterMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float verticalMovement = 0;

        if (Input.GetButton("Jump") && transform.position.y < waterSurfaceLevel - 0.5f)
        {
            verticalMovement = swimJumpSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            verticalMovement = -swimJumpSpeed;
        }
        else
        {
            verticalMovement = -passiveSinkSpeed;
        }

        moveDirection = new Vector3(horizontal, verticalMovement, vertical);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= swimSpeed;

        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);

        if (controller.isGrounded && transform.position.y >= waterSurfaceLevel)
        {
            SetInWater(false);
        }
        
        // Check if player is above water surface level
        else if (transform.position.y > waterSurfaceLevel)
        {
            Vector3 position = transform.position;
            position.y = waterSurfaceLevel;
            transform.position = position;
        }
    }

    public void SetInWater(bool inWater)
    {
        isInWater = inWater;
    }
}
