using UnityEngine;

public class ComportementProjectile : MonoBehaviour
{
    public float damage = 1f;
    public bool isCharging = true;  // True pendant la charge
    public bool canDealDamage = false;  // True seulement apr�s le lancement

    private AttaqueChara attaqueCharaScript; // Pour pr�venir la d�sint�gration c�t� joueur

    private void Start()
    {
        // Trouve le script AttaqueChara sur le joueur (ou un parent)
        attaqueCharaScript = FindObjectOfType<AttaqueChara>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCharging)
        {
            // En charge, touche quelque chose => d�sint�gration + annule la charge c�t� joueur
            if (attaqueCharaScript != null)
            {
                attaqueCharaScript.CancelChargeByProjectile();
            }
            Disintegrate();
        }
        else if (canDealDamage)
        {
            // Apr�s lancement, d�truit le projectile � l�impact, ici tu peux ajouter les d�g�ts

            // Exemple : si touche un ennemi (� adapter selon ton tag/structure)
            if (other.CompareTag("Enemy"))
            {
                // Tente de r�cup�rer un composant avec une m�thode TakeDamage
                var enemy = other.GetComponent<MonoBehaviour>();

                // V�rifie que l'objet a bien une m�thode TakeDamage(int) avant d'appeler
                if (enemy != null)
                {
                    // Utilisation de la r�flexion pour appeler TakeDamage si elle existe
                    var method = enemy.GetType().GetMethod("TakeDamage");
                    if (method != null)
                    {
                        method.Invoke(enemy, new object[] { (int)damage });
                    }
                    else
                    {
                        Debug.LogWarning("L'ennemi n'a pas de m�thode TakeDamage");
                    }
                }
            }

            Destroy(gameObject);
        }
        else
        {
            // S�curit�, d�truit toujours le projectile si impact sans d�g�ts
            Destroy(gameObject);
        }
    }

    public void Disintegrate()
    {
        Destroy(gameObject);
    }
}