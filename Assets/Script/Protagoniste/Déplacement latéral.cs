using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
    private bool isDashing = false;
    private float lastDashTime;
    private float moveInput;
    private Vector3 originalScale;

    [Header("Input System")]
    public InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction dashAction;

    //  Nouveau flag ajouté pour laisser le knockback faire son effet
    [HideInInspector] public bool isKnockedBack = false;

    void OnEnable()
    {
        if (inputActions == null)
        {
            return;
        }

        inputActions.Enable();
        moveAction = inputActions.FindAction("Player/Move", true);
        dashAction = inputActions.FindAction("Player/Dash", true);

        moveAction.performed += HandleMove;
        moveAction.canceled += StopMove;
        dashAction.started += HandleDash;

        moveAction.Enable();
        dashAction.Enable();
    }

    void OnDisable()
    {
        moveAction.performed -= HandleMove;
        moveAction.canceled -= StopMove;
        dashAction.started -= HandleDash;

        moveAction.Disable();
        dashAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        originalScale = transform.localScale;
    }

    void FixedUpdate()
    {
        //  On n'écrase pas la vélocité pendant un knockback
        if (!isDashing && !isKnockedBack)
        {
            rb.velocity = new Vector2(moveInput * maxSpeed, rb.velocity.y);
        }
    }

    private void Flip()
    {
        if (moveInput != 0) // Flip seulement si on se déplace
        {
            float newXScale = originalScale.x * Mathf.Sign(moveInput);
            transform.localScale = new Vector3(newXScale, originalScale.y, originalScale.z);
        }
    }

    private void HandleMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<float>();

        if (moveInput != 0)
        {
            Flip();
        }
    }

    private void StopMove(InputAction.CallbackContext ctx)
    {
        moveInput = 0;
        if (!isDashing && !isKnockedBack)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void HandleDash(InputAction.CallbackContext ctx)
    {
        if (!isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        float dashDirection = moveInput != 0 ? Mathf.Sign(moveInput) : transform.localScale.x;
        rb.velocity = new Vector2(dashDirection * dashForce, rb.velocity.y);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }
}