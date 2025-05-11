using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjEnemy : MonoBehaviour
{
    public float damage = 1f;
    public bool isCharging = true; // Devient false quand lancé

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Boule incantée a touché : {other.gameObject.name} (Tag: {other.tag})");

        if (isCharging)
        {
            // Se désagrège uniquement si ce n'est pas un ennemi
            if (!other.CompareTag("Enemy"))
            {
                Disintegrate();
            }
        }
        else
        {
            // Inflige des dégâts au joueur s'il touche la boule après son lancement
            if (other.CompareTag("Player"))
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    Debug.Log("Le joueur prend des dégâts en touchant la boule !");
                    playerHealth.TakeDamage(1, transform.position);
                }
                else
                {
                    Debug.LogError("Erreur : PlayerHealth non trouvé sur l'objet touché !");
                }
            }

            // Détruit la boule sauf si elle touche un ennemi
            if (!other.CompareTag("Enemy"))
            {
                Destroy(gameObject);
                Debug.Log("Boule incantée détruite !");
            }
        }
    }

    public void Disintegrate()
    {
        Debug.Log("Projectile : annulation de la charge !");
        Destroy(gameObject);
    }
}
