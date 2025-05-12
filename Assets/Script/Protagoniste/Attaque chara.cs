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

    private GameObject currentAttackInstance;
    private bool isCharging = false;
    private bool canShoot = false;
    private float currentChargeTime = 0f;
    private bool isOnCooldown = false;
    private bool materialChanged = false;
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Update()
    {
        HandleInput();
        UpdateProjectileAim();
    }

    void HandleInput()
    {
        Vector2 aimDirection = controls.Gameplay.Aim.ReadValue<Vector2>();

        // Correction : Vérifier `ChargeAttack` via `WasPressedThisFrame()`
        bool isChargingAttack = controls.Gameplay.ChargeAttack.WasPressedThisFrame();

        bool isAiming = aimDirection.magnitude > 0.1f;

        if (isChargingAttack && !isOnCooldown && isAiming && !isCharging)
        {
            StartCharging();
        }

        if (isChargingAttack && isCharging)
        {
            ChargeProjectile();
        }

        if (controls.Gameplay.ChargeAttack.WasReleasedThisFrame() && isCharging)
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
            Rigidbody2D projRb = currentAttackInstance.GetComponent<Rigidbody2D>();
            if (projRb != null)
            {
                projRb.velocity = direction.normalized * projectileSpeed;
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
        if (currentAttackInstance == null) return;

        Vector2 aimDirection = controls.Gameplay.Aim.ReadValue<Vector2>();

        if (aimDirection.magnitude < 0.1f) return;

        aimDirection.Normalize();

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        currentAttackInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

        float distance = 1.5f;
        currentAttackInstance.transform.position = (Vector2)attackSpawnPoint.position + aimDirection * distance;
    }

    //Ajout de la fonction manquante `SetProjectileMaterial()`
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