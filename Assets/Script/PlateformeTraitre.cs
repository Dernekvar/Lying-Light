using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateformeTraitre : MonoBehaviour
{
    [Header("Timings")]
    public float delayBeforeDisappear = 1f;  // Temps après contact avant disparition
    public float reappearDelay = 3f;         // Temps avant que la plateforme revienne

    private SpriteRenderer sr;
    private Collider2D col;
    private bool isTriggered = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
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
                    StartCoroutine(DisappearAndReappear());
                    isTriggered = true;
                    break;
                }
            }
        }
    }

    private IEnumerator DisappearAndReappear()
    {
        yield return new WaitForSeconds(delayBeforeDisappear);

        gameObject.SetActive(false); // Désactive complètement l'objet

        yield return new WaitForSeconds(reappearDelay);

        gameObject.SetActive(true); // Réactive complètement l'objet

        isTriggered = false; // Réactivation possible pour un autre passage
    }
}
