using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    public CharacterController controller;
    public Transform cameraTransform;

    public float speed = 10f;
    public float runSpeed = 20f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 2f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool isRunning;
    private PlayerStats playerStats;


    void OnEnable()
    {
        playerStats = GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found on Player!");
        }
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (UIState.IsUIOpen)
            return;
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (isRunning && !playerStats.CanSprint())
        {
            isRunning = false;
        }
        
        float currentSpeed = isRunning ? runSpeed : speed;

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 move = camRight * moveInput.x + camForward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false;
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Called by Input System
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (UIState.IsUIOpen)
            return;
        if (!isGrounded)
        {
            return;
        }
            
        if (context.started)
        {
            jumpPressed = true;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (playerStats == null)
            return;

        if (context.started && playerStats.CanSprint())
        {
            isRunning = true;
        }
        else if (context.canceled)
        {
            isRunning = false;
        }
    }

    public void OnShowTime(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            TimeLoopManager.Instance.ShowTime(true);
        }
        else if (context.canceled)
        {
            TimeLoopManager.Instance.ShowTime(false);
        }
    }

    public void Stun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    IEnumerator StunRoutine(float duration)
    {
        enabled = false;
        yield return new WaitForSeconds(duration);
        enabled = true;
    }

}