using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f; // Durée du dash
    public float dashCooldown = 1f;

    private float lastDashTime = -1f;
    private Rigidbody2D rb;
    private bool isDashing = false;
    private float lastMoveDirection = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleMovement();
        HandleDash();
    }

    void HandleMovement()
    {
        float moveInput = Input.GetKey(KeyCode.G) ? 1f : Input.GetKey(KeyCode.D) ? -1f : 0f;
        if (moveInput != 0) lastMoveDirection = moveInput;
        if (!isDashing)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        lastDashTime = Time.time;
        rb.velocity = new Vector2(lastMoveDirection * dashSpeed, rb.velocity.y);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
}