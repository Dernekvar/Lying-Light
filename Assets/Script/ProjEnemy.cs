using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjEnemy : MonoBehaviour
{
    public float damage = 1f;
    public bool isCharging = true; // Devient false quand lanc�

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Boule incant�e a touch� : {other.gameObject.name} (Tag: {other.tag})");

        if (isCharging)
        {
            // Se d�sagr�ge uniquement si ce n'est pas un ennemi
            if (!other.CompareTag("Enemy"))
            {
                Disintegrate();
            }
        }
        else
        {
            // Inflige des d�g�ts au joueur s'il touche la boule apr�s son lancement
            if (other.CompareTag("Player"))
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    Debug.Log("Le joueur prend des d�g�ts en touchant la boule !");
                    playerHealth.TakeDamage(1, transform.position);
                }
                else
                {
                    Debug.LogError("Erreur : PlayerHealth non trouv� sur l'objet touch� !");
                }
            }

            // D�truit la boule sauf si elle touche un ennemi
            if (!other.CompareTag("Enemy"))
            {
                Destroy(gameObject);
                Debug.Log("Boule incant�e d�truite !");
            }
        }
    }

    public void Disintegrate()
    {
        Debug.Log("Projectile : annulation de la charge !");
        Destroy(gameObject);
    }
}
