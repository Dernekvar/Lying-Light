using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateformeTraitre : MonoBehaviour
{
    [Header("Timings")]
    public float delayBeforeDisappear = 1f;  // Temps après contact avant disparition
    public float reappearDelay = 3f;         // Temps avant que la plateforme revienne
    public float intensityMultiplier = 2f;   // Intensité max de l'effet lumineux

    private SpriteRenderer sr;
    private Collider2D col;
    private bool isTriggered = false;
    private Color originalColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        originalColor = sr.color; // Sauvegarde la couleur originale
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Vérifie si c’est le joueur et qu’il atterrit par le haut
        if (!isTriggered && collision.collider.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.point.y > transform.position.y + 0.1f) // marge de sécurité
                {
                    StartCoroutine(FadeToDisappear());
                    isTriggered = true;
                    break;
                }
            }
        }
    }

    private IEnumerator FadeToDisappear()
    {
        float elapsedTime = 0f;
        float blinkInterval = 0.05f; // Départ avec clignotement rapide

        while (elapsedTime < delayBeforeDisappear)
        {
            elapsedTime += blinkInterval; // Augmente le temps écoulé
            sr.enabled = !sr.enabled; // Alterne l'affichage

            // Augmente progressivement l'intervalle du clignotement
            blinkInterval *= 1.2f;

            yield return new WaitForSeconds(blinkInterval);
        }

        sr.enabled = false; // Une fois le délai écoulé, la plateforme disparaît

        StartCoroutine(DisappearAndReappear());
    }


    private IEnumerator DisappearAndReappear()
    {
        sr.enabled = false;

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D c in colliders)
        {
            c.enabled = false;
        }

        yield return new WaitForSeconds(reappearDelay);

        // Réinitialisation de la couleur avant réapparition
        sr.color = originalColor;
        sr.enabled = true;

        foreach (Collider2D c in colliders)
        {
            c.enabled = true;
        }

        isTriggered = false;
    }
}