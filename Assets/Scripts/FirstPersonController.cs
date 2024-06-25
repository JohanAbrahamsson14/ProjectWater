using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(WaterEffects))]
public class FirstPersonController : MonoBehaviour
{
    public float speed = 6.0f;
    public float runningSpeed = 8.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;

    public float staminaBase = 10f;
    private float staminaCurrent;
    public float staminaRefreshRate = 4f;
    public float staminaUsageRate = 6f;

    public float swimSpeed = 3.0f;
    public float swimRunningSpeed = 3.5f;
    public float swimJumpSpeed = 4.0f;
    public float swimGravity = 2.0f;  // Adjusted for a more realistic sinking effect
    public float passiveSinkSpeed = 1.0f;  // Speed at which the player sinks passively
    public float waterSurfaceLevel = 5.0f; // Example water surface level, adjust as needed
    public float Weight = 80f;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private bool isInWater = false;
    public bool isGrabbed = false;
    
    public GameObject fogWall;

    private WaterEffects waterEffects;
    
    private HashSet<KeyCode> keysToCheck = new HashSet<KeyCode>(){ KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

    void Start()
    {
        controller = GetComponent<CharacterController>();
        waterEffects = GetComponent<WaterEffects>();
        staminaCurrent = staminaBase;
        SetInWater(true);
    }

    void Update()
    {
        staminaCurrent += staminaRefreshRate * Time.deltaTime;
        if(Input.GetKey(KeyCode.LeftShift)) staminaCurrent -= staminaUsageRate * Time.deltaTime;
        staminaCurrent = Mathf.Clamp(staminaCurrent, 0, staminaBase);
        fogWall.SetActive(isInWater);

        if (isGrabbed)
        {
            GrabbedMovement();
        }
        else if (isInWater)
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
            moveDirection *= Input.GetKey(KeyCode.LeftShift)&& staminaCurrent>0?runningSpeed:speed;

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
            float horizontalSpeed = (Input.GetKey(KeyCode.LeftShift)&& staminaCurrent>0 ? swimRunningSpeed:swimSpeed);
            float verticalSpeed = (Input.GetKey(KeyCode.LeftShift)&& staminaCurrent>0 ? swimRunningSpeed:swimSpeed);

            float horizontal = Input.GetAxis("Horizontal") * horizontalSpeed;
            float vertical = Input.GetAxis("Vertical") * verticalSpeed;

            float verticalMovement = 0;

            if (Input.GetButton("Jump"))
            {
                verticalMovement = swimJumpSpeed;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                verticalMovement = -swimJumpSpeed;
            }

            Vector3 horizontalMovement = new Vector3(horizontal, 0, vertical);
            horizontalMovement = transform.TransformDirection(horizontalMovement);

            // Applying the vertical movement separately
            Vector3 verticalDirection = new Vector3(0, verticalMovement, 0);

            // Combining both movements
            moveDirection = horizontalMovement + verticalDirection;
            moveDirection.y -= swimGravity* (Weight*Weight/6400);
            // Apply movement to the controller
            controller.Move(moveDirection * Time.deltaTime);




        /*
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
        */
    }

    void GrabbedMovement()
    {
    }
    
    public void SetInWater(bool inWater)
    {
        isInWater = inWater;

        if (isInWater)
        {
            waterEffects.WaterEffectActive();
        }
        else
        {
            waterEffects.WaterEffectDisactive();
        }
        
    }
    
    public void SetGrabbed(bool grabbed)
    {
        isGrabbed = grabbed;
    }

    public float GrabbedValue()
    { 
        float value = 0;
        
        float mouseMoveValue = 3;
        float keyboardValue = 1;
        float mouseClickValue = 2;
        
        if(Input.GetAxis("Mouse X")<0){
            value += mouseMoveValue*Time.deltaTime;
        }
        if(Input.GetAxis("Mouse X")>0){
            value += mouseMoveValue*Time.deltaTime;
        }
        
        foreach (var key in keysToCheck)
        {
            if(Input.GetKeyDown(key)) value += keyboardValue;
        }
        
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            value += mouseClickValue;
        }

        return value;
    }
}
