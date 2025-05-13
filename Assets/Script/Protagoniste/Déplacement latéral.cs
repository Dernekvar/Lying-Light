using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveForce = 10f;
    public float maxSpeed = 5f;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
    private bool isDashing = false;
    private float lastDashTime;
    private float moveInput;

    [Header("Input System")]
    public InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction dashAction;

    void OnEnable()
    {
        if (inputActions == null)
        {
            Debug.LogError(" inputActions est NULL ! Vérifie son assignation dans l'Inspector.");
            return;
        }
        inputActions.Enable();
        moveAction = inputActions.FindAction("Player/Move", true);
        dashAction = inputActions.FindAction("Player/Dash", true);

        Debug.Log(" Vérification Move & Dash : " +
                  "Move Action: " + (moveAction != null ? "OK" : "Manquant") +
                  "Dash Action: " + (dashAction != null ? "OK" : "Manquant"));

        moveAction.performed += HandleMove;
        dashAction.started += HandleDash;

        moveAction.Enable();
        dashAction.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
        moveAction.performed -= HandleMove;
        dashAction.started -= HandleDash;

        moveAction.Disable();
        dashAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    private void HandleMove(InputAction.CallbackContext ctx)
    {
        Vector2 moveInputVector = ctx.ReadValue<Vector2>();
        moveInput = moveInputVector.x;

        Debug.Log(" Déplacement détecté : " + moveInputVector);

        if (!isDashing)
        {
            rb.velocity = new Vector2(moveInput*moveForce, rb.velocity.y);
            //rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y);
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