using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationPlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.point.y > transform.position.y) // Vérifie que le joueur atterrit bien dessus
                {
                    // Parcourir les enfants de la plateforme
                    foreach (Transform child in transform)
                    {
                        if (child.CompareTag("Enemy")) // Vérifie si l'enfant a le tag "Enemy"
                        {
                            // Récupère n'importe quel script qui contient la méthode Activer()
                            MonoBehaviour enemyScript = child.GetComponent<MonoBehaviour>();
                            if (enemyScript != null)
                            {
                                // Vérifie si le script possède la méthode Activer()
                                System.Reflection.MethodInfo methodInfo = enemyScript.GetType().GetMethod("Activer");

                                if (methodInfo != null)
                                {
                                    methodInfo.Invoke(enemyScript, null); // Exécute la méthode Activer()
                                }
                            }
                        }
                    }

                    break;
                }
            }
        }
    }
}