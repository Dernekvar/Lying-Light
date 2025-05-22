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
    public float orbitRadius = 1.5f;

    [Header("Auto-Launch Settings")]
    public float autoLaunchTime = 1f; // Temps en secondes avant le tir automatique une fois chargée
    private float timeSinceReady = 0f;

    public bool isCharging = false;
    public bool canShoot = false;
    private GameObject currentAttackInstance;
    public float currentChargeTime = 0f;
    public bool isOnCooldown = false;
    public bool materialChanged = false;

    private Vector2 aimDirection = Vector2.right;
    private Vector2 defaultAimDirection = new Vector2(1, 0);
    private PlayerInput playerInput;
    private InputAction aimAction;
    private InputAction chargeAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        aimAction = playerInput.actions["Aim"];
        chargeAction = playerInput.actions["ChargeAttack"];
    }

    void Update()
    {
        HandleAim();
        HandleChargeAttack();

        if (isCharging && currentAttackInstance != null)
        {
            currentAttackInstance.transform.position = attackSpawnPoint.position;
        }
    }

    void HandleAim()
    {
        if (aimAction == null || attackSpawnPoint == null)
        {
            Debug.LogError("AimAction ou AttackSpawnPoint est null !");
            return;
        }

        aimDirection = aimAction.ReadValue<Vector2>();

        if (aimDirection.sqrMagnitude < 0.001f)
            aimDirection = defaultAimDirection;

        aimDirection.Normalize();

        attackSpawnPoint.position = transform.position + (Vector3)(aimDirection * orbitRadius);

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        attackSpawnPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void HandleChargeAttack()
    {
        if (chargeAction.WasPressedThisFrame() && !isOnCooldown && !isCharging)
        {
            StartCharging();
        }

        if (chargeAction.IsPressed() && isCharging && currentAttackInstance != null)
        {
            currentChargeTime += Time.deltaTime;
            float progress = Mathf.Clamp01(currentChargeTime / chargeTime);
            float scaleValue = Mathf.Lerp(0.3f, targetScale, progress);
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

        // Déclencher le tir automatique après x secondes une fois chargé
        if (canShoot)
        {
            timeSinceReady += Time.deltaTime;
            if (timeSinceReady >= autoLaunchTime)
            {
                LaunchProjectile();
                timeSinceReady = 0f; // Reset le timer
                StartCoroutine(CooldownCoroutine());
                currentChargeTime = 0f;
            }
        }

        if (chargeAction.WasReleasedThisFrame() && isCharging)
        {
            if (canShoot)
            {
                LaunchProjectile();
            }
            else
            {
                Destroy(currentAttackInstance);
                isCharging = false;
                canShoot = false;
                isOnCooldown = true;
                StartCoroutine(CooldownCoroutine());
                currentChargeTime = 0f;
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
                projRb.velocity = aimDirection * projectileSpeed;
            }

            currentAttackInstance.transform.parent = null;
            currentAttackInstance = null;
        }

        StartCoroutine(CooldownCoroutine());
        currentChargeTime = 0f;
    }

    public void StartCooldown()
    {
        isOnCooldown = true;
        StartCoroutine(CooldownCoroutine());
    }


    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }
}