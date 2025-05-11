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
                    // Trouver tous les objets ayant le tag "Enemy" et v�rifier leur �tat
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (var enemy in enemies)
                    {
                        // R�cup�re n'importe quel script qui contient la m�thode Activer()
                        MonoBehaviour enemyScript = enemy.GetComponent<MonoBehaviour>();
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

                    break;
                }
            }
        }
    }
}