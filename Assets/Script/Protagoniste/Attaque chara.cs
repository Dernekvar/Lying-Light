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

    [Header("Input System")]
    public InputActionAsset inputActions;
    private InputAction aimAction;
    private InputAction chargeAction;

    void OnEnable()
    {
        aimAction = inputActions.FindAction("Player/Aim", true);
        chargeAction = inputActions.FindAction("Player/ChargeAttack", true);

        aimAction.performed += HandleAim;
        chargeAction.started += HandleAttack;
        chargeAction.performed += HandleCharge;
        chargeAction.canceled += HandleRelease;

        aimAction.Enable();
        chargeAction.Enable();
    }

    void OnDisable()
    {
        aimAction.performed -= HandleAim;
        chargeAction.started -= HandleAttack;
        chargeAction.performed -= HandleCharge;
        chargeAction.canceled -= HandleRelease;

        aimAction.Disable();
        chargeAction.Disable();
    }

    private void HandleAim(InputAction.CallbackContext ctx)
    {
        if (currentAttackInstance == null) return;

        Vector2 aimDirection = ctx.ReadValue<Vector2>();
        if (aimDirection.magnitude < 0.1f) return;

        aimDirection.Normalize();
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        currentAttackInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

        float distance = 1.5f;
        currentAttackInstance.transform.position = (Vector2)attackSpawnPoint.position + aimDirection * distance;
    }

    private void HandleAttack(InputAction.CallbackContext ctx)
    {
        if (!isOnCooldown && !isCharging)
        {
            StartCharging();
        }
    }

    private void HandleCharge(InputAction.CallbackContext ctx)
    {
        if (currentAttackInstance == null) return;

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

    private void HandleRelease(InputAction.CallbackContext ctx)
    {
        if (isCharging)
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

    private void StartCharging()
    {
        isCharging = true;
        canShoot = false;
        materialChanged = false;
        currentChargeTime = 0f;

        currentAttackInstance = Instantiate(projectilePrefab, attackSpawnPoint.position, Quaternion.identity);
        currentAttackInstance.transform.localScale = Vector3.zero;

        SetProjectileMaterial(chargingMaterial);
    }

    private void SetProjectileMaterial(Material mat)
    {
        if (currentAttackInstance == null || mat == null) return;

        SpriteRenderer sr = currentAttackInstance.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.material = mat;
        }
    }

    private void LaunchProjectile()
    {
        isCharging = false;
        isOnCooldown = true;

        if (currentAttackInstance != null)
        {
            Rigidbody2D projRb = currentAttackInstance.GetComponent<Rigidbody2D>();
            if (projRb != null)
            {
                Vector2 aimDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                projRb.velocity = aimDirection.normalized * projectileSpeed;
            }

            currentAttackInstance.transform.parent = null;
            currentAttackInstance = null;
        }

        StartCoroutine(CooldownCoroutine());
        currentChargeTime = 0f;
    }

    private void DisintegrateProjectile()
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
}