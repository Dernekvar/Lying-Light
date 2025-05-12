using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Déplacement avec le joystick gauche
        moveInput = Input.GetAxis("Horizontal");

        // LT = généralement mappé à "3rd axis" / "Axis 9" selon manette
        float ltValue = Input.GetAxis("LT"); // Tu dois avoir ce mapping dans Edit > Project Settings > Input Manager

        // Optionnel : Debug de la valeur LT
        // Debug.Log("LT Value: " + ltValue);

        // Dash uniquement si on est pas déjà en train de dasher et que le cooldown est terminé
        if (!isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            bool dashTriggered = Input.GetKeyDown(KeyCode.J) || ltValue > 0.5f;

            if (dashTriggered)
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