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
    public HealthUI healthUI; // Référence au script HealthUI
    public int damageOnCollision = 1; // Définir la quantité de dégâts par défaut

    public float knockbackForce = 10f;
    public float invincibilityTime = 1.5f;
    public float blinkInterval = 0.1f;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        healthUI.UpdateHearts(); // Met à jour les cœurs au début
    }

    public void TakeDamage(int amount, Vector2 sourcePosition)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        Debug.Log("Player took damage. HP: " + currentHealth);
        healthUI.UpdateHearts();

        CancelChargingProjectiles(); // Annule et supprime les projectiles encore en charge

        Vector2 knockbackDir = (transform.position - (Vector3)sourcePosition).normalized;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(Invincibility());

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    void CancelChargingProjectiles()
    {
        ComportementProjectile[] projectiles = FindObjectsOfType<ComportementProjectile>();
        foreach (var proj in projectiles)
        {
            if (proj.isCharging) // Seuls les projectiles en charge sont concernés
            {
                Destroy(proj.gameObject); // Supprime le prefab de la scène
            }
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision détectée avec : " + collision.gameObject.name);

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Dégâts infligés : " + damageOnCollision);
            TakeDamage(damageOnCollision, collision.transform.position);
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
