using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateformeTraitre : MonoBehaviour
{
    [Header("Timings")]
    public float delayBeforeDisappear = 1f;  // Temps apr�s contact avant disparition
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
        // V�rifie si c�est le joueur et qu�il atterrit par le haut
        if (!isTriggered && collision.collider.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.point.y > transform.position.y + 0.1f) // marge de s�curit�
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

        gameObject.SetActive(false); // D�sactive compl�tement l'objet

        yield return new WaitForSeconds(reappearDelay);

        gameObject.SetActive(true); // R�active compl�tement l'objet

        isTriggered = false; // R�activation possible pour un autre passage
    }
}
