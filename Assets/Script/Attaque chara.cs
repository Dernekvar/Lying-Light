using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attaquechara : MonoBehaviour
{
    public GameObject attackPrefab; // Préfab de l'attaque
    public Transform attackSpawnPoint; // Point de départ de l'attaque
    public float chargeTime = 1f; // Temps pour atteindre une charge complète
    public float maxChargeTime = 3f; // Temps total pour atteindre la charge maximale
    public float[] recoilForces = { 500f, 1000f, 1500f }; // Forces de recul pour chaque niveau de charge
    public Vector3 maxAttackScale = new Vector3(2f, 2f, 2f); // Échelle maximale de l'attaque
    public float attackRadius = 2f; // Rayon de la zone circulaire

    private float currentChargeTime = 0f;
    private int chargeLevel = 0;
    private bool isCharging = false;
    private Rigidbody2D rb;
    private GameObject currentAttackInstance;
    private Vector2 originalVelocity;
    private float originalAngularVelocity;
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
        if (Input.GetKey(KeyCode.K))
        {
            if (!isCharging)
            {
                isCharging = true;
                currentChargeTime = 0f;
                currentAttackInstance = Instantiate(attackPrefab, attackSpawnPoint.position, attackSpawnPoint.rotation);

                // Sauvegarde la vélocité actuelle et fige le Rigidbody2D
                originalVelocity = rb.velocity;
                originalAngularVelocity = rb.angularVelocity;
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
        if (Input.GetKeyUp(KeyCode.K) && chargeLevel > 0)
        {
            LaunchAttack();
        }
    }

    void LaunchAttack()
    {
        if (currentAttackInstance != null)
        {
            // Calcule la direction vers la position de la souris
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 launchDirection = (mousePosition - (Vector2)attackSpawnPoint.position).normalized;

            Rigidbody2D attackRb = currentAttackInstance.GetComponent<Rigidbody2D>();
            if (attackRb != null)
            {
                attackRb.velocity = launchDirection * 10f; // Ajustez la vitesse selon vos besoins
            }

            // Applique le recul au personnage
            Vector2 recoilDirection = -launchDirection;
            rb.AddForce(recoilDirection * recoilForces[chargeLevel], ForceMode2D.Impulse);

            // Réinitialise la charge et restaure les contraintes du Rigidbody2D
            currentChargeTime = 0f;
            chargeLevel = 0;
            currentAttackInstance = null;
            rb.constraints = originalConstraints;
            rb.velocity = originalVelocity;
            rb.angularVelocity = originalAngularVelocity;
        }
    }

    void UpdateAttackPositionAndRotation()
    {
        // Calcule la direction vers la position de la souris
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - (Vector2)attackSpawnPoint.position).normalized;

        // Calcule la position de l'attaque dans la zone circulaire
        Vector2 attackPosition = attackSpawnPoint.position+direction * attackRadius;
        currentAttackInstance.transform.position = attackPosition;

        // Met à jour la rotation de l'attaque pour qu'elle pointe vers la souris
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        currentAttackInstance.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}