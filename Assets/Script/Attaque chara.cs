using System.Collections;
using UnityEngine;

public class Attaquechara : MonoBehaviour
{
    public GameObject attackPrefab;
    public Transform attackSpawnPoint;
    public float chargeTime = 1f;
    public float maxChargeTime = 3f;
    public float[] recoilForces = { 500f, 1000f, 1500f };
    public Vector3 maxAttackScale = new Vector3(2f, 2f, 2f);
    public float attackRadius = 2f;

    private float currentChargeTime = 0f;
    private int chargeLevel = 0;
    private bool isCharging = false;
    private Rigidbody2D rb;
    private GameObject currentAttackInstance;
    private Vector2 originalVelocity;
    private RigidbodyConstraints2D originalConstraints;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component is missing from the player.");
        }
    }

    void Update()
    {
        HandleCharging();
        HandleAttack();

        if (isCharging && currentAttackInstance != null)
        {
            UpdateAttackPositionAndRotation();
        }
    }

    void HandleCharging()
    {
        if (Input.GetMouseButton(1))
        {
            if (!isCharging)
            {
                isCharging = true;
                currentChargeTime = 0f;
                currentAttackInstance = Instantiate(attackPrefab, attackSpawnPoint.position, attackSpawnPoint.rotation);

                originalVelocity = rb.velocity;
                originalConstraints = rb.constraints;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            else
            {
                currentChargeTime += Time.deltaTime;
                chargeLevel = Mathf.Min(Mathf.FloorToInt(currentChargeTime / chargeTime), recoilForces.Length - 1);
                float scaleFactor = Mathf.Clamp01(currentChargeTime / maxChargeTime);
                currentAttackInstance.transform.localScale = Vector3.Lerp(Vector3.one, maxAttackScale, scaleFactor);
            }
        }
        else if (isCharging)
        {
            isCharging = false;
            LaunchAttack();
        }
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonUp(1) && chargeLevel > 0)
        {
            LaunchAttack();
        }
    }

    void LaunchAttack()
    {
        if (currentAttackInstance != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 launchDirection = (mousePosition - (Vector2)attackSpawnPoint.position).normalized;

            Rigidbody2D attackRb = currentAttackInstance.GetComponent<Rigidbody2D>();
            if (attackRb != null)
            {
                attackRb.velocity = launchDirection * 10f;
            }

            rb.constraints = originalConstraints;

            Vector2 recoilDirection = -launchDirection;
            if (rb != null && chargeLevel >= 0 && chargeLevel < recoilForces.Length)
            {
                rb.AddForce(recoilDirection * recoilForces[chargeLevel], ForceMode2D.Force);

                GetComponent<PlayerMovement>()?.ApplyRecoil(1f);
            }

            currentChargeTime = 0f;
            chargeLevel = 0;
            currentAttackInstance = null;
        }
    }

    void UpdateAttackPositionAndRotation()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)attackSpawnPoint.position).normalized;

        Vector2 attackPosition = (Vector2)attackSpawnPoint.position + direction * attackRadius;
        currentAttackInstance.transform.position = attackPosition;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        currentAttackInstance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
