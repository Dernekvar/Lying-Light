using System.Collections;
using UnityEngine;

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

    private GameObject currentAttackInstance;
    private bool isCharging = false;
    private bool canShoot = false;
    private float currentChargeTime = 0f;
    private bool isOnCooldown = false;
    private bool materialChanged = false;

    void Update()
    {
        HandleInput();
        UpdateProjectileAim();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && !isOnCooldown)
        {
            StartCharging();
        }

        if (Input.GetMouseButton(0) && isCharging)
        {
            ChargeProjectile();
        }

        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            if (canShoot)
            {
                LaunchProjectile();
            }
            else
            {
                DisintegrateProjectile();
            }
        }
    }

    void StartCharging()
    {
        isCharging = true;
        canShoot = false;
        materialChanged = false;
        currentChargeTime = 0f;

        currentAttackInstance = Instantiate(projectilePrefab, attackSpawnPoint.position, Quaternion.identity);
        currentAttackInstance.transform.localScale = Vector3.zero;

        // Appliquer le matériau de chargement dès le départ
        SetProjectileMaterial(chargingMaterial);
    }

    void ChargeProjectile()
    {
        if (currentAttackInstance == null)
        {
            isCharging = false;
            currentChargeTime = 0f;
            return;
        }

        currentChargeTime += Time.deltaTime;
        float progress = Mathf.Clamp01(currentChargeTime / chargeTime);
        float scaleValue = Mathf.Lerp(0f, targetScale, progress);
        currentAttackInstance.transform.localScale = new Vector3(scaleValue, scaleValue, 1f);

        if (scaleValue >= targetScale)
        {
            canShoot = true;

            // Change le matériau une seule fois
            if (!materialChanged)
            {
                SetProjectileMaterial(readyMaterial);
                materialChanged = true;
            }
        }
    }

    void LaunchProjectile()
    {
        isCharging = false;
        isOnCooldown = true;

        ComportementProjectile projectileScript = currentAttackInstance.GetComponent<ComportementProjectile>();
        if (projectileScript != null)
        {
            projectileScript.damage = damage;
            projectileScript.isCharging = false;
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

    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
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

    void SetProjectileMaterial(Material mat)
    {
        if (currentAttackInstance == null || mat == null) return;

        SpriteRenderer sr = currentAttackInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.material = mat;
        }
    }
}
