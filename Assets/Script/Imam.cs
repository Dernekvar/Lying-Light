using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imam : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float chargeTime = 2f;
    public float targetScale = 2f;
    public float projectileSpeed = 10f;
    public float damage = 1f;

    [Header("Cooldown Settings")]
    public float cooldownTime = 3f;

    [Header("Visual Settings")]
    public Material chargingMaterial;
    public Material readyMaterial;

    private GameObject currentEnergyBall;
    private bool isActive = false;
    private bool canShoot = true;

    public void Activer()
    {
        if (isActive) return; // Empêche une réactivation multiple
        Debug.Log($"{name} - ACTIVATION : L'ennemi reste actif !");

        isActive = true;
        StartCoroutine(IncantationLoop());
    }

    private IEnumerator IncantationLoop()
    {
        while (isActive) // Tant que l'ennemi est vivant, il continue d'incanter
        {
            yield return StartCoroutine(Incantation());
            yield return new WaitForSeconds(cooldownTime); // Attente avant la prochaine attaque
        }
    }

    private IEnumerator Incantation()
    {
        if (!canShoot) yield break;

        Debug.Log($"{name} - INCANTATION DÉMARRÉE !");
        canShoot = false;

        // Instanciation de la boule d'énergie
        currentEnergyBall = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        currentEnergyBall.transform.localScale = Vector3.zero;
        SetProjectileMaterial(chargingMaterial);

        float currentChargeTime = 0f;
        while (currentChargeTime < chargeTime)
        {
            currentChargeTime += Time.deltaTime;
            float progress = Mathf.Pow(currentChargeTime / chargeTime, 1.5f);
            float scaleValue = Mathf.Lerp(0f, targetScale, progress);
            currentEnergyBall.transform.localScale = new Vector3(scaleValue, scaleValue, 1f);

            yield return null;
        }

        SetProjectileMaterial(readyMaterial);
        Debug.Log($"{name} - Boule prête, lancement !");
        TirProjectile();
        canShoot = true;
    }

    private void TirProjectile()
    {
        if (currentEnergyBall == null) return;

        GameObject joueur = GameObject.FindGameObjectWithTag("Player");
        if (joueur != null)
        {
            Vector2 directionTir = (joueur.transform.position - transform.position).normalized;
            Rigidbody2D projRb = currentEnergyBall.GetComponent<Rigidbody2D>();
            if (projRb != null)
            {
                projRb.velocity = directionTir * projectileSpeed;
            }

            Debug.Log($"{name} - Projectile lancé vers {joueur.name} !");
        }

        currentEnergyBall.transform.parent = null;
        currentEnergyBall = null;
    }

    void SetProjectileMaterial(Material mat)
    {
        if (currentEnergyBall == null || mat == null) return;

        SpriteRenderer sr = currentEnergyBall.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.material = mat;
        }
    }


    public void Mourir() // Fonction à appeler pour "tuer" l'ennemi
    {
        Debug.Log($"{name} - L'ennemi est mort !");
        isActive = false;
        StopAllCoroutines();
        Destroy(gameObject);
    }
}