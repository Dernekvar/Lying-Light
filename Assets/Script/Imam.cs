using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imam : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float chargeTime = 2f;
    public float targetScale = 2f;
    public float projectileSpeed = 10f;
    public float damage = 1f;

    [Header("Cooldown Settings")]
    public float cooldownTime = 3f;

    [Header("Visual Settings")]
    public Material chargingMaterial;
    public Material readyMaterial;

    [Header("Vie")]
    public int maxHealth = 2;
    private int currentHealth;

    private GameObject currentEnergyBall;
    private bool isActive = false;
    private bool canShoot = true;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    public void Activer()
    {
        if (isActive) return;
        Debug.Log($"{name} - ACTIVATION : L'ennemi reste actif !");
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

        Debug.Log($"{name} - INCANTATION DÉMARRÉE !");
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
            yield return null;
        }

        if (currentEnergyBall != null)
        {
            SetProjectileMaterial(readyMaterial);
            Debug.Log($"{name} - Boule prête, lancement !");
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

            Debug.Log($"{name} - Projectile lancé vers {joueur.name} !");
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{name} - Prend {damage} dégât(s). PV restants : {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{name} - L'Imam est mort !");
        isActive = false;
        StopAllCoroutines();

        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerProjectile"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }
}
