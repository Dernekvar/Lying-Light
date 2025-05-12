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
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Déplacement avec Left Stick
        //moveInput = controls.Gameplay.Move.ReadValue<Vector2>().x;
        Debug.Log("Move Input: " + controls.Gameplay.Move.ReadValue<Vector2>().x);

        // Dash avec Left Trigger (L2)
        if (!isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            if (controls.Gameplay.Dash.WasPressedThisFrame())
            {
                StartCoroutine(Dash());
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            if (Mathf.Abs(rb.velocity.x) < maxSpeed)
            {
                rb.AddForce(new Vector2(moveInput * moveForce, 0f), ForceMode2D.Force);
            }
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;

        float dashDirection = moveInput != 0 ? Mathf.Sign(moveInput) : transform.localScale.x;
        rb.velocity = new Vector2(dashDirection * dashForce, rb.velocity.y);

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
    }
}