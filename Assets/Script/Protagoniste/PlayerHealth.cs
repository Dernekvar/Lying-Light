using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public float knockbackForce = 10f;
    public float invincibilityTime = 1f;
    public float blinkInterval = 0.1f;
    public int damageOnCollision = 1;
    public Transform playerSpawnPoint;

    private bool isInvincible = false;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private PlayerMovement playerMovement;
    private Collider2D playerCollider;

    public HealthUI healthUI;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCollider = GetComponent<Collider2D>();
    }

    public void TakeDamage(int amount, Vector2 sourcePosition)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        Debug.Log("Player took damage. HP: " + currentHealth);
        healthUI.UpdateHearts();


        // Récupérer le point de collision à partir du collider
        Vector2 collisionPoint = playerCollider != null
            ? playerCollider.ClosestPoint(sourcePosition)
            : transform.position;

        // Calcul du knockback directionnel
        Vector2 knockbackDir = ((Vector2)transform.position - collisionPoint).normalized;

        // Renforce le knockback horizontal, limite le vertical
        if (Mathf.Abs(knockbackDir.x) < 0.01f)
            knockbackDir.x = 1f; // évite x = 0
        else
            knockbackDir.x = Mathf.Sign(knockbackDir.x) * Mathf.Max(Mathf.Abs(knockbackDir.x), 1f);

        knockbackDir.y = Mathf.Sign(knockbackDir.y) * Mathf.Min(Mathf.Abs(knockbackDir.y), 0.5f);

        Debug.Log("Knockback direction: " + knockbackDir);

        // Appliquer le knockback
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

        // Activer l'invincibilité visuelle + logique
        StartCoroutine(Invincibility());

        // Désactive temporairement le contrôle horizontal
        StartCoroutine(HandleKnockback());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HandleKnockback()
    {
        if (playerMovement != null)
        {
            playerMovement.isKnockedBack = true;
        }

        yield return new WaitForSeconds(0.2f); // Durée du knockback

        if (playerMovement != null)
        {
            playerMovement.isKnockedBack = false;
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision détectée avec : " + collision.gameObject.name);

        if (collision.CompareTag("Checkpoint"))
        {
            playerSpawnPoint = collision.transform; // Met à jour le point de spawn
            Debug.Log("Nouveau point de spawn défini : " + playerSpawnPoint.position);
        }

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
        int defaultLayer = gameObject.layer;

        while (timer < invincibilityTime)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
            gameObject.layer = LayerMask.NameToLayer("Invincible");
        }

        sr.enabled = true;
        isInvincible = false;
        gameObject.layer = defaultLayer;
    }
    private IEnumerator FadeOutAndRespawn()
    {
        // Fade-out du SpriteRenderer
        float fadeDuration = 1f; // Durée du fade-out en secondes
        float elapsedTime = 0f;
        Color startColor = sr.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            sr.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }

        // Réinitialiser l'opacité du SpriteRenderer
        sr.color = startColor;

        // Réapparition du joueur au point de spawn
        transform.position = playerSpawnPoint.position;

        // Réinitialiser la scène
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Die()
    {
        Debug.Log("Player is dead.");
        StartCoroutine(FadeOutAndRespawn());
    }
}