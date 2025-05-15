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

    private GameObject currentAttackInstance;
    private bool isCharging = false;
    private bool canShoot = false;
    private float currentChargeTime = 0f;
    private bool isOnCooldown = false;
    private bool materialChanged = false;

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

    void Flip()
    {
        // Inversion de l'échelle du personnage
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        // Inversion de la direction de visée
        aimDirection = -defaultAimDirection;
    }

    void HandleAim()
    {
        // Vérification pour éviter une NullReferenceException
        if (aimAction == null || attackSpawnPoint == null)
        {
            Debug.LogError("AimAction ou AttackSpawnPoint est null !");
            return;
        }

        aimDirection = aimAction.ReadValue<Vector2>();

        // Si le joystick n'est pas utilisé, conserver la direction par défaut
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

        if (chargeAction.WasReleasedThisFrame() && isCharging)
        {
            if (canShoot)
                LaunchProjectile();
            else
                DisintegrateProjectile();
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, orbitRadius);
    }
}