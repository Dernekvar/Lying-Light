using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attaquecollider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // D�truit l'attaque lorsqu'elle touche un autre collider
        Destroy(gameObject);
    }
}