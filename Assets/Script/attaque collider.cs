using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attaquecollider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Détruit l'attaque lorsqu'elle touche un autre collider
        Destroy(gameObject);
    }
}