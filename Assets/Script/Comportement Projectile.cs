using UnityEngine;

public class ComportementProjectile : MonoBehaviour
{
    public float damage = 1f;
    public bool isCharging = true; // Devient false quand lancé

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCharging)
        {
            // Se désagrège s'il touche quelque chose pendant la charge
            Disintegrate();
        }
        else
        {
           
            Destroy(gameObject); // Le projectile disparaît à l’impact
        }
    }

    void Disintegrate()
    {
        // Optionnel : effet visuel
        Destroy(gameObject);
    }
}
