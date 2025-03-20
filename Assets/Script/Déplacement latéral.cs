using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;       // Vitesse de déplacement
    public float dashSpeed = 20f;      // Vitesse du dash
    public float dashDistance = 5f;    // Distance du dash
    public float dashCooldown = 1f;    // Temps de récupération entre chaque dash
    private float lastDashTime = -1f;  // Dernier moment où un dash a été effectué
    private Rigidbody2D rb;
    private bool isDashing = false;    // Indique si le personnage est en train de dasher
    private float lastMoveDirection = 1f;  // Dernière direction de mouvement (1 pour droite, -1 pour gauche)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // Récupère le Rigidbody2D du joueur
    }

    void Update()
    {
        Move();  // Gère le mouvement du personnage

        // Vérifie si le cooldown est passé et si la touche "J" est pressée pour lancer le dash
        if (!isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            if (Input.GetKeyDown(KeyCode.J))  // Si on appuie sur "J" pour dasher
            {
                Dash();  // Lance le dash
            }
        }
    }

    // Déplacement du joueur
    void Move()
    {
        float moveInput = 0f;

        // Déplacement à droite avec la touche "G"
        if (Input.GetKey(KeyCode.G))
        {
            moveInput = 1f;
            lastMoveDirection = 1f;  // Enregistre la direction droite
        }
        // Déplacement à gauche avec la touche "D"
        else if (Input.GetKey(KeyCode.D))
        {
            moveInput = -1f;
            lastMoveDirection = -1f; // Enregistre la direction gauche
        }

        // Applique la vitesse de déplacement uniquement horizontalement
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    // Fonction pour effectuer le dash
    void Dash()
    {
        // Enregistre le moment du dernier dash
        lastDashTime = Time.time;
        isDashing = true;

        // Applique une impulsion dans la direction du dash (en fonction de la dernière direction)
        rb.velocity = new Vector2(lastMoveDirection * dashSpeed, rb.velocity.y);

        // Affichage de debug pour vérifier si le dash fonctionne
        Debug.Log("Dash effectué en direction " + (lastMoveDirection == 1f ? "droite" : "gauche"));

        // On arrête le dash après avoir parcouru la distance voulue
        StartCoroutine(StopDashAfterDistance());
    }

    // Coroutine pour arrêter le dash après avoir parcouru la distance du dash
    private IEnumerator StopDashAfterDistance()
    {
        Vector2 startPosition = rb.position;
        while (Vector2.Distance(startPosition, rb.position) < dashDistance)
        {
            yield return null; // Attends le prochain frame
        }

        // Quand le joueur a parcouru la distance, on arrête le dash
        isDashing = false;
        rb.velocity = new Vector2(0, rb.velocity.y);  // Arrête le déplacement horizontal
    }
}
