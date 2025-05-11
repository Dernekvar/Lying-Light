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
                    // Trouver tous les objets ayant le tag "Enemy" et vérifier leur état
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (var enemy in enemies)
                    {
                        // Récupère n'importe quel script qui contient la méthode Activer()
                        MonoBehaviour enemyScript = enemy.GetComponent<MonoBehaviour>();
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

                    break;
                }
            }
        }
    }
}