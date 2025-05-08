using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationPlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log($"{name} - Joueur a atterri sur la plateforme, activation des enfants.");

            // Active tous les enfants Enfant dans la hiérarchie
            Enfant[] enfants = GetComponentsInChildren<Enfant>();
            foreach (var enfant in enfants)
            {
                enfant.Activer();
            }
        }
    }
}
