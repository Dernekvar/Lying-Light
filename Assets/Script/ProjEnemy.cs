using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjEnemy : MonoBehaviour
{
    public float damage = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Boule incant�e a touch� : {other.gameObject.name} (Tag: {other.tag})");

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

            // D�truit la boule apr�s avoir inflig� des d�g�ts au joueur
            Destroy(gameObject);
            Debug.Log("Boule incant�e d�truite apr�s avoir touch� le joueur !");
        }

        // D�truit la boule � l'impact, sauf si elle touche un ennemi
        if (!other.CompareTag("Enemy") || other.CompareTag("PlayerProjectile"))
        {
            Destroy(gameObject);
            Debug.Log("Boule incant�e d�truite !");
        }
    }
}