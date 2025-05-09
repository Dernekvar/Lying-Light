using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    private bool isInvincible = false;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    public HealthUI healthUI; // R�f�rence au script HealthUI

    public float knockbackForce = 10f;
    public float invincibilityTime = 1.5f;
    public float blinkInterval = 0.1f;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        healthUI.UpdateHearts(); // Met � jour les c�urs au d�but
    }

    public void TakeDamage(int amount, Vector2 sourcePosition)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        Debug.Log("Player took damage. HP: " + currentHealth);
        healthUI.UpdateHearts(); // Met � jour les c�urs apr�s avoir subi des d�g�ts

        // Knockback
        Vector2 knockbackDir = (transform.position - (Vector3)sourcePosition).normalized;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        // Invincibilit� + clignotement
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
        // Animation / mort / �cran de game over � ajouter ici
    }
}