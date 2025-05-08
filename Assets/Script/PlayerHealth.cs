using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;
    private bool isInvincible = false;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    public float knockbackForce = 10f;
    public float invincibilityTime = 1.5f;
    public float blinkInterval = 0.1f;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount, Vector2 sourcePosition)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        Debug.Log("Player took damage. HP: " + currentHealth);

        // Knockback
        Vector2 knockbackDir = (transform.position - (Vector3)sourcePosition).normalized;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        // Invincibilité + clignotement
        StartCoroutine(Invincibility());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;
        float timer = 0f;

        while (timer < invincibilityTime)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        sr.enabled = true;
        isInvincible = false;
    }

    private void Die()
    {
        Debug.Log("Player is dead.");
        // Animation / mort / écran de game over à ajouter ici
    }
}
