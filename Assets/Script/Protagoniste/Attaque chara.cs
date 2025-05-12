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
        {
            Debug.Log("Stick Droit : " + Input.GetAxis("RightStickHorizontal") + ", " + Input.GetAxis("RightStickVertical"));
            Debug.Log("RT : " + Input.GetAxis("RT"));
        }
        HandleInput();
        UpdateProjectileAim();
    }

    void HandleInput()
    {
        // Joystick droit
        Vector2 aimDirection = new Vector2(Input.GetAxis("RightStickHorizontal"), Input.GetAxis("RightStickVertical"));
        float rtValue = Input.GetAxis("RT"); // Gâchette droite

        bool isAiming = aimDirection.magnitude > 0.1f;

        if (rtValue > 0.1f && !isOnCooldown && isAiming && !isCharging)
        {
            StartCharging();
        }

        if (rtValue > 0.1f && isCharging)
        {
            ChargeProjectile();
        }

        if (rtValue <= 0.1f && isCharging)
        {
            if (canShoot)
            {
                LaunchProjectile(aimDirection);
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
            if (!materialChanged)
            {
                SetProjectileMaterial(readyMaterial);
                materialChanged = true;
            }
        }
    }

    void LaunchProjectile(Vector2 direction)
    {
        isCharging = false;
        isOnCooldown = true;

        if (currentAttackInstance != null)
        {
            ComportementProjectile projectileScript = currentAttackInstance.GetComponent<ComportementProjectile>();
            if (projectileScript != null)
            {
                projectileScript.damage = damage;
                projectileScript.isCharging = false;
            }

            direction.Normalize();

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

        Vector2 aimDirection = new Vector2(Input.GetAxis("RightStickHorizontal"), Input.GetAxis("RightStickVertical"));

        if (aimDirection.magnitude < 0.1f)
            return;

        aimDirection.Normalize();

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        currentAttackInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

        float distance = 1.5f;
        currentAttackInstance.transform.position = (Vector2)attackSpawnPoint.position + aimDirection * distance;
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