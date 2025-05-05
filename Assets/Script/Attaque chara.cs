using System.Collections;
using UnityEngine;

public class AttaqueChara : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform attackSpawnPoint;
    public float chargeTime = 2f;              // Temps pour atteindre la taille maximale
    public float targetScale = 2f;             // Taille finale
    public float projectileSpeed = 10f;

    private GameObject currentAttackInstance;
    private bool isCharging = false;
    private bool canShoot = false;  // Si l'attaque peut être lancée
    private float currentChargeTime = 0f;
    private Rigidbody2D rb;
    private RigidbodyConstraints2D originalConstraints;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInput();
        UpdateProjectileAim();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCharging();
        }

        if (Input.GetMouseButton(0) && isCharging)
        {
            ChargeProjectile();
        }

        if (Input.GetMouseButtonUp(0) && isCharging && canShoot)
        {
            LaunchProjectile();
        }
    }

    void StartCharging()
    {
        isCharging = true;
        canShoot = false;  // On désactive le tir avant que le projectile soit complètement chargé
        currentChargeTime = 0f;

        // Freeze la position verticale du joueur pour le garder suspendu en l'air
        if (rb != null)
        {
            originalConstraints = rb.constraints;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        currentAttackInstance = Instantiate(projectilePrefab, attackSpawnPoint.position, Quaternion.identity);
        currentAttackInstance.transform.localScale = Vector3.zero;
    }

    void ChargeProjectile()
    {
        currentChargeTime += Time.deltaTime;
        float progress = Mathf.Clamp01(currentChargeTime / chargeTime);

        // Met à jour la taille du projectile
        float scaleValue = Mathf.Lerp(0f, targetScale, progress);
        currentAttackInstance.transform.localScale = new Vector3(scaleValue, scaleValue, 1f);

        // Si le projectile a atteint la taille cible, on permet de le tirer
        if (scaleValue >= targetScale)
        {
            canShoot = true;
        }
    }

    void LaunchProjectile()
    {
        isCharging = false;

        if (rb != null)
        {
            rb.constraints = originalConstraints; // Déverrouille le joueur
        }

        if (currentAttackInstance != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2)attackSpawnPoint.position).normalized;

            Rigidbody2D projRb = currentAttackInstance.GetComponent<Rigidbody2D>();
            if (projRb != null)
            {
                projRb.velocity = direction * projectileSpeed;
            }

            currentAttackInstance.transform.parent = null;
            currentAttackInstance = null;
        }

        currentChargeTime = 0f;
    }

    void UpdateProjectileAim()
    {
        if (currentAttackInstance == null)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)attackSpawnPoint.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        currentAttackInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

        float distance = 1.5f;
        currentAttackInstance.transform.position = (Vector2)attackSpawnPoint.position + direction * distance;
    }
}
