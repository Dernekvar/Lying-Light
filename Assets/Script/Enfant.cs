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
        Debug.Log($"{name} - Enfant initialisé, inactif.");
    }

    void Update()
    {
        if (!isActive) return;

        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        Vector2 checkPos = new Vector2(groundCheck.position.x, groundCheck.position.y);
        if (!Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer))
        {
            direction *= -1;
            Flip();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      if (collision.gameObject.CompareTag("Player"))
        {
            print("contact");
        }
    }

    public void Activer()
    {
        isActive = true;
        Debug.Log($"{name} - Enfant activé via plateforme.");
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
