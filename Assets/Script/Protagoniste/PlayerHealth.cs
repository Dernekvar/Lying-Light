using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Assurez-vous d'inclure ce namespace

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    private bool isInvincible = false;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    public HealthUI healthUI; // R�f�rence au script HealthUI
    public int damageOnCollision = 1; // D�finir la quantit� de d�g�ts par d�faut
    public Transform playerSpawnPoint; // R�f�rence au point de spawn du joueur

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
            if (proj.isCharging) // Seuls les projectiles en charge sont concern�s
            {
                Destroy(proj.gameObject); // Supprime le prefab de la sc�ne
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision d�tect�e avec : " + collision.gameObject.name);

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("D�g�ts inflig�s : " + damageOnCollision);
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
            gameObject.layer = LayerMask.NameToLayer("Invincible");
        }

        sr.enabled = true;
        isInvincible = false;
    }

    private IEnumerator FadeOutAndRespawn()
    {
        // Fade-out du SpriteRenderer
        float fadeDuration = 1f; // Dur�e du fade-out en secondes
        float elapsedTime = 0f;
        Color startColor = sr.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            sr.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }

        // R�initialiser l'opacit� du SpriteRenderer
        sr.color = startColor;

        // R�apparition du joueur au point de spawn
        transform.position = playerSpawnPoint.position;

        // R�initialiser la sc�ne
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Die()
    {
        Debug.Log("Player is dead.");
        StartCoroutine(FadeOutAndRespawn());
    }
}