using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCharacter : MonoBehaviour
{
    public float jumpHeight = 5f;
    private Rigidbody2D rb;
    private bool isGrounded;
    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Vérifie si le joueur est au sol et si la touche "R" est pressée
        if (isGrounded && Input.GetKeyDown(KeyCode.R))
        {
            Jump();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Vérifie si la collision est avec un objet de la couche "groundLayer"
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Vérifie si la collision est avec un objet de la couche "groundLayer"
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }

    void Jump()
    {
        // Applique une force de saut
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
    }
}