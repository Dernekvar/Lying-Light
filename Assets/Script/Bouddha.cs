using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouddha : MonoBehaviour
{
    public enum MovementState { Infinity, Dashing, Returning }

    [Header("Dégâts et Activation")]
    public float damage = 1f;
    private bool isActive = false;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float chargeTime = 2f;
    public float targetScale = 2f;
    public float projectileSpeed = 10f;
    public float cooldownTime = 3f;
    private bool canShoot = true;
    private GameObject currentEnergyBall;

    [Header("Visual Settings")]
    public Material chargingMaterial;
    public Material readyMaterial;

    [Header("Déplacement en infini")]
    public float infinitySpeed = 2f;
    public float infinityAmplitude = 1f;
    public float infinityFrequency = 1f;
    public Transform centerPoint;

    [Header("Retour au centre")]
    public float returnSpeed = 3f;

    [Header("Charge vers joueur")]
    public float dashSpeed = 6f;
    public float attackInterval = 5f;

    private MovementState currentState = MovementState.Infinity;

    private float attackTimer;
    private float timeSinceStart;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color originalColor;

    [Header("Vie")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Clignotement Dégâts")]
    public float flashDuration = 1f;
    public float flashInterval = 0.1f;

    private GameObject currentProjectile;

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
        if (!canShoot) yield break;

        canShoot = false;

        currentEnergyBall = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        currentEnergyBall.transform.localScale = Vector3.zero;
        SetProjectileMaterial(chargingMaterial);

        float currentChargeTime = 0f;
        while (currentChargeTime < chargeTime && currentEnergyBall != null)
        {
            currentChargeTime += Time.deltaTime;
            float progress = Mathf.Pow(currentChargeTime / chargeTime, 1.5f);
            float scaleValue = Mathf.Lerp(0f, targetScale, progress);

            currentEnergyBall.transform.localScale = new Vector3(scaleValue, scaleValue, 1f);
            currentEnergyBall.transform.position = firePoint.position; // ici pour suivre la position pendant la charge

            yield return null;
        }

        if (currentEnergyBall != null)
        {
            SetProjectileMaterial(readyMaterial);
            TirProjectile();
        }

        canShoot = true;
    }

    private void TirProjectile()
    {
        if (currentEnergyBall == null) return;

        GameObject joueur = GameObject.FindGameObjectWithTag("Player");
        if (joueur != null)
        {
            Vector2 directionTir = (joueur.transform.position - transform.position).normalized;
            Rigidbody2D projRb = currentEnergyBall.GetComponent<Rigidbody2D>();
            if (projRb != null)
            {
                projRb.velocity = directionTir * projectileSpeed;
            }
        }

        currentEnergyBall.transform.parent = null;
        currentEnergyBall = null;
    }

    void SetProjectileMaterial(Material mat)
    {
        if (currentEnergyBall == null || mat == null) return;

        SpriteRenderer sr = currentEnergyBall.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.material = mat;
        }
    }

    void Update()
    {
        if (!isActive) return;

        attackTimer += Time.deltaTime;

        switch (currentState)
        {
            case MovementState.Infinity:
                MoveInInfinity(); // **Ta fonction intacte**
                if (attackTimer >= attackInterval)
                {
                    attackTimer = 0f;
                    StartCoroutine(DashTowardsPlayer());
                }
                break;

            case MovementState.Dashing:
                // On ne change pas rb.velocity ici, il est défini dans DashTowardsPlayer
                break;

            case MovementState.Returning:
                ReturnToCenter();
                break;
        }
    }

    void MoveInInfinity()
    {
        timeSinceStart += Time.deltaTime * infinityFrequency;
        float x = Mathf.Sin(timeSinceStart) * infinityAmplitude;
        float y = Mathf.Sin(timeSinceStart * 2) * infinityAmplitude;
        Vector2 target = (Vector2)centerPoint.position + new Vector2(x, y);
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        rb.velocity = dir * infinitySpeed;
    }

    IEnumerator DashTowardsPlayer()
    {
        currentState = MovementState.Dashing;

        GameObject joueur = GameObject.FindGameObjectWithTag("Player");
        if (joueur != null)
        {
            Vector2 dir = (joueur.transform.position - transform.position).normalized;
            rb.velocity = dir * dashSpeed;
        }
        else
        {
            currentState = MovementState.Returning; // Pas de joueur, retourne au centre
        }

        yield return null;
    }

    void ReturnToCenter()
    {
        Vector2 dir = (centerPoint.position - transform.position);
        if (dir.magnitude < 0.1f)
        {
            currentState = MovementState.Infinity;
            timeSinceStart = 0f;
            rb.velocity = Vector2.zero;
            return;
        }
        rb.velocity = dir.normalized * returnSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == MovementState.Dashing)
        {
            currentState = MovementState.Returning;
        }
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