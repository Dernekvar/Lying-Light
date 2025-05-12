using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Enfant : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    private bool isActive = false;
    private int direction = 1;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public int maxHealth = 1; // Vie maximale de l'ennemi
    private int currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth; // Initialiser la vie de l'ennemi
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy took damage. HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FadeOutAndDie()
    {
        // Arrêter le déplacement
        rb.velocity = Vector2.zero;
        rb.isKinematic = true; // Facultatif : désactiver la physique si besoin

        // Fade-out du SpriteRenderer
        float fadeDuration = 1f;
        float elapsedTime = 0f;
        Color startColor = sr.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            sr.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerProjectile"))
        {
            TakeDamage(1); // Tu peux ajuster la valeur si tu veux que certains projectiles fassent plus de dégâts
            Destroy(collision.gameObject); // On détruit le projectile après l’impact
        }
    }

    private void Die()
    {
        Debug.Log("Enemy is dead.");

        // Geler les positions X et Y
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

        StartCoroutine(FadeOutAndDie());
    }
}