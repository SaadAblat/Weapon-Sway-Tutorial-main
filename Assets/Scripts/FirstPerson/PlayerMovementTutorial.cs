using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEditor;

public class PlayerMovementTutorial : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float maxVerticalSpeed;

    public float groundDrag;
    public float airDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    bool jumpRequested;

    public float jetPush;
    bool isJetPushing;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    [Header("Animations And Effects")]


    bool hasLanded;
    public float land_Shake_Intensity;
    public float land_Shake_Frequency;
    public float land_Shake_Time;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);


        
        LandingEffect();
        MyInput();

    }
    private void LandingEffect()
    {
        if (grounded)
        {
            if (!hasLanded)
            {
                CinemachineShake.CameraInstance.ShakeCamera(land_Shake_Intensity, land_Shake_Time, land_Shake_Frequency);
                // 
                hasLanded = true;
            }
        }
        else
        {
            hasLanded = false;
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
        if (isJetPushing)
        {
            JetPackPush();
        }
        if (jumpRequested)
        {
            jumpRequested = false;
            Jump();
        }
        SpeedControl();
        // handle drag
        ManageDrag();
        FallMultiplier();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

          
            jumpRequested = true;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        if (Input.GetKey(jumpKey))
        {
         
            isJetPushing = true;
        }
        else
        {
            isJetPushing = false;

        }
    }

    private void JetPackPush()
    {
        rb.AddForce(transform.up * jetPush, ForceMode.Acceleration);
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }
    private void ManageDrag()
    {
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = airDrag;
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
        Vector3 verticalVel = new Vector3(0f, rb.velocity.y, 0);
        if (flatVel.magnitude > maxVerticalSpeed)
        {
            Vector3 limitedVel = verticalVel.normalized * maxVerticalSpeed;
            rb.velocity = new Vector3(rb.velocity.x, limitedVel.y, rb.velocity.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }


    [SerializeField] float fallMultiplier= 5;
    private void FallMultiplier()
    {
        if (!grounded && rb.velocity.y < -1f && !isJetPushing)
        {
            rb.velocity += fallMultiplier * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }


    }
}