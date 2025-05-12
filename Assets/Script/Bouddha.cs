using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouddha : MonoBehaviour
{
    [Header("Dégâts et Activation")]
    public float damage = 1f;
    private bool isActive = false;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float chargeTime = 2f;
    public float cooldownTime = 3f;

    [Header("Déplacement")]
    public float speed = 3f;
    private Rigidbody2D rb;
    private Vector2 direction;

    [Header("Gestion de blocage")]
    private float stuckTimer = 0f;
    private float checkInterval = 1f;
    private float minSpeedThreshold = 0.5f;
    private bool isIntangible = false;
    private float intangibleDuration = 2f;

    [Header("Vie")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Clignotement Dégâts")]
    public float flashDuration = 1f;
    public float flashInterval = 0.1f;

    private GameObject currentProjectile;
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        currentHealth = maxHealth;
    }

    public void Activer()
    {
        if (isActive) return;

        isActive = true;
        StartCoroutine(IncantationLoop());

        GameObject joueur = GameObject.FindGameObjectWithTag("Player");
        if (joueur != null)
        {
            direction = (joueur.transform.position - transform.position).normalized;
            rb.velocity = direction * speed;
        }
    }

    private IEnumerator IncantationLoop()
    {
        while (isActive)
        {
            yield return StartCoroutine(Incantation());
            yield return new WaitForSeconds(cooldownTime);
        }
    }

    private IEnumerator Incantation()
    {
        currentProjectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        currentProjectile.transform.localScale = Vector3.zero;

        Rigidbody2D projRb = currentProjectile.GetComponent<Rigidbody2D>();

        float currentChargeTime = 0f;
        while (currentChargeTime < chargeTime && currentProjectile != null)
        {
            currentChargeTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentChargeTime / chargeTime);
            float scaleValue = Mathf.Lerp(0f, 2f, progress);
            currentProjectile.transform.localScale = new Vector3(scaleValue, scaleValue, 1f);

            currentProjectile.transform.position = firePoint.position;

            yield return null;
        }

        if (currentProjectile != null)
        {
            GameObject joueur = GameObject.FindGameObjectWithTag("Player");
            if (joueur != null)
            {
                Vector2 directionTir = (joueur.transform.position - transform.position).normalized;
                projRb.velocity = directionTir * 5f;
            }

            currentProjectile = null;
        }
    }

    private void Update()
    {
        if (!isActive) return;

        if (rb.velocity.magnitude < minSpeedThreshold)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= checkInterval)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
                int obstacleCount = 0;

                foreach (Collider2D col in colliders)
                {
                    if (col.CompareTag("Enemy") || col.CompareTag("Obstacle"))
                        obstacleCount++;
                }

                if (obstacleCount >= 2)
                {
                    StartCoroutine(ActiverIntangibilite());
                }
                else
                {
                    direction = Random.insideUnitCircle.normalized;
                    rb.velocity = direction * speed;
                }

                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    private IEnumerator ActiverIntangibilite()
    {
        if (isIntangible) yield break;

        isIntangible = true;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        GameObject joueur = GameObject.FindGameObjectWithTag("Player");
        if (joueur != null)
        {
            direction = (joueur.transform.position - transform.position).normalized;
            rb.velocity = direction * speed * 1.5f;
        }

        yield return new WaitForSeconds(intangibleDuration);

        if (col != null) col.enabled = true;
        isIntangible = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject joueur = GameObject.FindGameObjectWithTag("Player");

        if (joueur != null)
        {
            direction = (joueur.transform.position - transform.position).normalized;
        }
        else
        {
            float angle = Random.Range(-60f, 60f);
            direction = Quaternion.Euler(0, 0, angle) * direction;
        }

        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(1, transform.position);
        }
        else if (collision.CompareTag("PlayerProjectile"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRed()
    {
        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(flashInterval / 2f);
            sr.color = originalColor;
            yield return new WaitForSeconds(flashInterval / 2f);
            elapsed += flashInterval;
        }

        sr.color = originalColor;
    }

    private void Die()
    {
        isActive = false;
        StopAllCoroutines();

        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

        if (currentProjectile != null)
            Destroy(currentProjectile);

        StartCoroutine(FadeOutAndDie());
    }

    private IEnumerator FadeOutAndDie()
    {
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
}