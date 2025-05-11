using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Enfant : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    private bool isActive = false;
    private int direction = 1;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (!isActive) return;

        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        Vector2 checkPos = new Vector2(groundCheck.position.x, groundCheck.position.y);
        bool isGrounded = Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer);


        if (!isGrounded)
        {
            direction *= -1;
            Flip();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            direction *= -1; // Inverser la direction
            Flip(); // Appliquer l'effet visuel
        }
    }

    public void Activer()
    {
        isActive = true;
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
