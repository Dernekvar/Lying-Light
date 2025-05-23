using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Créditdefin : MonoBehaviour
{
    public GameObject creditsPanel; // Panel des crédits
    public float fadeDuration = 2f; // Durée du fade-in
    public GameObject targetEnemy; // Ennemi spécifique dont la mort déclenche les crédits

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = creditsPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup manquant sur le panel des crédits !");
            return;
        }

        creditsPanel.SetActive(false);
        canvasGroup.alpha = 0f;
    }

    public void RegisterKill(GameObject enemy)
    {
        if (enemy == targetEnemy)
        {
            StartCoroutine(FadeInCredits());
        }
    }

    private IEnumerator FadeInCredits()
    {
        creditsPanel.SetActive(true);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
