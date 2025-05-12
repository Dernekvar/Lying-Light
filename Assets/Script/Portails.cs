using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portails : MonoBehaviour
{
    [Header("Configuration")]
    public Transform spawnPoint;
    public int numberOfEnemies = 3;
    public float spawnInterval = 1f;
    public List<GameObject> enemyPrefabs; // Liste de prefabs des ennemis à instancier

    private bool isActivated = false;

    // Méthode appelée par la plateforme d'activation
    public void Activer()
    {
        ActiverPortail(); // Permet au système d'appel générique de fonctionner (via Reflection)
    }

    // Active le portail et commence le spawn
    public void ActiverPortail()
    {
        if (isActivated) return;

        isActivated = true;
        StartCoroutine(SpawnEnemies());
    }

    // Coroutine qui spawn les ennemis à intervalles réguliers
    private IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            enemyInstance.transform.localScale = Vector3.one; // Correction ici

            if (enemyInstance.TryGetComponent(out Enfant enfant))
            {
                enfant.Activer();
            }
            else if (enemyInstance.TryGetComponent(out Imam imam))
            {
                imam.Activer();
            }
            else if (enemyInstance.TryGetComponent(out Bouddha bouddha))
            {
                bouddha.Activer();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
