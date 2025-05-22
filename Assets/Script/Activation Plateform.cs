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
                if (contact.point.y > transform.position.y) // V�rifie que le joueur atterrit bien dessus
                {
                    // Parcourir les enfants de la plateforme
                    foreach (Transform child in transform)
                    {
                        if (child.CompareTag("Enemy")) // V�rifie si l'enfant a le tag "Enemy"
                        {
                            // R�cup�re n'importe quel script qui contient la m�thode Activer()
                            MonoBehaviour enemyScript = child.GetComponent<MonoBehaviour>();
                            if (enemyScript != null)
                            {
                                // V�rifie si le script poss�de la m�thode Activer()
                                System.Reflection.MethodInfo methodInfo = enemyScript.GetType().GetMethod("Activer");

                                if (methodInfo != null)
                                {
                                    methodInfo.Invoke(enemyScript, null); // Ex�cute la m�thode Activer()
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