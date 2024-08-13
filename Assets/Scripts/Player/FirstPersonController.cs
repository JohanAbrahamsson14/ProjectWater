using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.VFX;
using Random = System.Random;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(WaterEffects))]
[RequireComponent(typeof(Interactor))]
public class FirstPersonController : MonoBehaviour, IDamageAble
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

    public VisualEffect blood;
    public VisualEffect bloodPassive;
    public VisualEffect bloodInWater;
    public VisualEffect bloodInWaterPassive;
    public float bloodEffectChange;
    
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private bool isInWater = false;
    public bool isGrabbed = false;
    
    public GameObject fogWall;

    private WaterEffects waterEffects;
    
    private HashSet<KeyCode> keysToCheck = new HashSet<KeyCode>(){ KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

    public float playerMaxHealth;

    private Camera _camera;
    private Interactor _interactor;
    public float interactRange;

    /*
    public List<Item> inventory;
    public Item activeItem;
    */
    public Inventory inventory;
    public GameObject activeItemObject;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        waterEffects = GetComponent<WaterEffects>();
        _interactor = GetComponent<Interactor>();
        _camera = GetComponentInChildren<Camera>();
        staminaCurrent = staminaBase;
        SetInWater(true);
        maxHealth = playerMaxHealth;
        currentHealth = maxHealth;
    }

    void Update()
    {
        staminaCurrent += staminaRefreshRate * Time.deltaTime;
        if(Input.GetKey(KeyCode.LeftShift)) staminaCurrent -= staminaUsageRate * Time.deltaTime;
        staminaCurrent = Mathf.Clamp(staminaCurrent, 0, staminaBase);
        fogWall.SetActive(isInWater);

        
        if (Input.GetMouseButtonDown(0))
        {
            if (inventory.activeItem != null && inventory.activeItem is Tool activeTool)
            {
                activeTool.ToolAction();
            }
        }
        
        
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            _interactor.Interact(_camera.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0)), interactRange, this);
        }
        if (Input.GetKeyDown(KeyCode.G) && inventory.inventoryCollection.Count > 0)
        {
            Item selectedItem = inventory.inventoryCollection.Last();
            GameObject selectedItemObject = selectedItem.itemObject;
            selectedItemObject.transform.position = transform.position+Vector3.down;
            Weight -= selectedItem.weight;
            selectedItemObject.SetActive(true);
            inventory.inventoryCollection.Remove(selectedItem);
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                _interactor.Interact(_camera.ViewportPointToRay(new Vector3 (0.5f, 0.5f, 0)), interactRange, this);
            }
            if (Input.GetKeyDown(KeyCode.G) && inventory.inventoryCollection.Count > 0)
            {
                Item selectedItem = inventory.inventoryCollection.Last();
                GameObject selectedItemObject = selectedItem.itemObject;
                selectedItemObject.transform.position = transform.position+Vector3.down;
                Weight -= selectedItem.weight;
                selectedItemObject.SetActive(true);
                inventory.inventoryCollection.Remove(selectedItem);
            }


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
    
    public float maxHealth { get; set; }
    public float currentHealth { get; set; }
    public void GetDamaged(float value)
    {
        if(bloodEffectChange<value) bloodInWater.SendEvent("OnPlayBlood");
        bloodInWaterPassive.SetFloat("Bleeding Amount", 1-(currentHealth/maxHealth));
        currentHealth -= value;
        if (currentHealth <= 0) Death();
    }

    public void Death()
    {
       Debug.Log("dead");
    }
}
