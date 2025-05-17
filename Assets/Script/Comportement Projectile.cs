using UnityEngine;

public class ComportementProjectile : MonoBehaviour
{
    public float damage = 1f;
    public bool isCharging = true;  // True pendant la charge
    public bool canDealDamage = false;  // True seulement après le lancement

    private AttaqueChara attaqueCharaScript; // Pour prévenir la désintégration côté joueur

    private void Start()
    {
        // Trouve le script AttaqueChara sur le joueur (ou un parent)
        attaqueCharaScript = FindObjectOfType<AttaqueChara>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCharging)
        {
            // En charge, touche quelque chose => désintégration + annule la charge côté joueur
            if (attaqueCharaScript != null)
            {
                attaqueCharaScript.CancelChargeByProjectile();
            }
            Disintegrate();
        }
        else if (canDealDamage)
        {
            // Après lancement, détruit le projectile à l’impact, ici tu peux ajouter les dégâts

            // Exemple : si touche un ennemi (à adapter selon ton tag/structure)
            if (other.CompareTag("Enemy"))
            {
                // Tente de récupérer un composant avec une méthode TakeDamage
                var enemy = other.GetComponent<MonoBehaviour>();

                // Vérifie que l'objet a bien une méthode TakeDamage(int) avant d'appeler
                if (enemy != null)
                {
                    // Utilisation de la réflexion pour appeler TakeDamage si elle existe
                    var method = enemy.GetType().GetMethod("TakeDamage");
                    if (method != null)
                    {
                        method.Invoke(enemy, new object[] { (int)damage });
                    }
                    else
                    {
                        Debug.LogWarning("L'ennemi n'a pas de méthode TakeDamage");
                    }
                }
            }

            Destroy(gameObject);
        }
        else
        {
            // Sécurité, détruit toujours le projectile si impact sans dégâts
            Destroy(gameObject);
        }
    }

    public void Disintegrate()
    {
        Destroy(gameObject);
    }
}