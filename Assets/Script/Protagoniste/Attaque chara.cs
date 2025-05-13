using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttaqueChara : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform attackSpawnPoint;
    public float chargeTime = 2f;
    public float targetScale = 2f;
    public float projectileSpeed = 10f;
    public float damage = 1f;

    [Header("Cooldown Settings")]
    public float cooldownTime = 1.5f;

    [Header("Visual Settings")]
    public Material chargingMaterial;
    public Material readyMaterial;

    [Header("Orbit Settings")]
    public float orbitRadius = 1.5f; // Distance fixe autour du joueur

    private GameObject currentAttackInstance;
    private bool isCharging = false;
    private bool canShoot = false;
    private float currentChargeTime = 0f;
    private bool isOnCooldown = false;
    private bool materialChanged = false;

    void Update()
    {
        HandleMouseInput();
        UpdateAttackSpawnPoint();

        // Si une attaque est en charge, la position de l'attaque doit suivre le spawn point
        if (isCharging && currentAttackInstance != null)
        {
            currentAttackInstance.transform.position = attackSpawnPoint.position;
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0) && !isOnCooldown && !isCharging)
        {
            StartCharging();
        }

        if (Input.GetMouseButton(0) && isCharging && currentAttackInstance != null)
        {
            currentChargeTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentChargeTime / chargeTime);
            float scaleValue = Mathf.Lerp(0f, targetScale, progress);
            currentAttackInstance.transform.localScale = new Vector3(scaleValue, scaleValue, 1f);

            if (scaleValue >= targetScale)
            {
                canShoot = true;
                if (!materialChanged)
                {
                    SetProjectileMaterial(readyMaterial);
                    materialChanged = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            if (canShoot)
                LaunchProjectile();
            else
                DisintegrateProjectile();
        }
    }

    void UpdateAttackSpawnPoint()
    {
        // Obtenir la position de la souris en monde
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
        );

        Vector2 direction = (mouseWorldPos - transform.position);

        if (direction.sqrMagnitude < 0.001f)
        {
            direction = Vector2.right; // Direction par défaut si la souris est trop proche
        }
        else
        {
            direction.Normalize(); // Sinon on normalise normalement
        }

        attackSpawnPoint.position = transform.position + (Vector3)(direction * orbitRadius);

        // Le faire pointer vers la souris
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        attackSpawnPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void StartCharging()
    {
        isCharging = true;
        canShoot = false;
        materialChanged = false;
        currentChargeTime = 0f;

        currentAttackInstance = Instantiate(projectilePrefab, attackSpawnPoint.position, Quaternion.identity);
        currentAttackInstance.transform.localScale = Vector3.zero;

        SetProjectileMaterial(chargingMaterial);
    }

    void SetProjectileMaterial(Material mat)
    {
        if (currentAttackInstance == null || mat == null) return;

        SpriteRenderer sr = currentAttackInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.material = mat;
        }
    }

    void LaunchProjectile()
    {
        isCharging = false;
        isOnCooldown = true;

        if (currentAttackInstance != null)
        {
            Rigidbody2D projRb = currentAttackInstance.GetComponent<Rigidbody2D>();
            if (projRb != null)
            {
                Vector2 aimDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
                projRb.velocity = aimDirection * projectileSpeed;
            }

            currentAttackInstance.transform.parent = null;
            currentAttackInstance = null;
        }

        StartCoroutine(CooldownCoroutine());
        currentChargeTime = 0f;
    }

    void DisintegrateProjectile()
    {
        isCharging = false;

        if (currentAttackInstance != null)
        {
            Destroy(currentAttackInstance);
            currentAttackInstance = null;
        }

        currentChargeTime = 0f;
        isOnCooldown = true;
        StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

    void OnDrawGizmosSelected()
    {
        // Visualise le cercle autour du joueur
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, orbitRadius);
    }
}