using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private float dashTimer = 0f;
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
        if (isDashing)
            return;

        float moveInput = Input.GetKey(KeyCode.G) ? 1f : Input.GetKey(KeyCode.D) ? -1f : 0f;

        if (moveInput != 0)
            lastMoveDirection = moveInput;

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            isDashing = true;
            lastDashTime = Time.time;
            dashTimer = dashDuration;
            rb.velocity = new Vector2(lastMoveDirection * dashSpeed, rb.velocity.y);
        }

        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }
    }
}
