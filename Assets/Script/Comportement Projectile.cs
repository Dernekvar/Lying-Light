using UnityEngine;

public class ComportementProjectile : MonoBehaviour
{
    public float damage = 1f;
    public bool isCharging = true; // Devient false quand lanc�

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCharging)
        {
            // Se d�sagr�ge s'il touche quelque chose pendant la charge
            Disintegrate();
        }
        else
        {
           
            Destroy(gameObject); // Le projectile dispara�t � l�impact
        }
    }

    void Disintegrate()
    {
        // Optionnel : effet visuel
        Destroy(gameObject);
    }
}
