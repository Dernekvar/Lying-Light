using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disintegrate : MonoBehaviour
{
    private AttaqueChara attaqueChara;

    private void Start()
    {
        attaqueChara = FindObjectOfType<AttaqueChara>();
        if (attaqueChara == null)
        {
            Debug.LogError("AttaqueChara non trouv� !");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (attaqueChara == null) return;

        // Cas : le projectile �tait en cours de charge (pas encore pr�t)
        if (attaqueChara.isCharging && !attaqueChara.canShoot)
        {
            attaqueChara.isCharging = false;
            attaqueChara.canShoot = false;
            attaqueChara.currentChargeTime = 0f;
            attaqueChara.materialChanged = false;

            attaqueChara.StartCooldown(); // on lance le cooldown depuis le script d'origine
            Destroy(gameObject);
        }

        // Cas : le projectile �tait d�j� pr�t (canShoot), mais a �t� d�truit sans �tre lanc�
        else if (attaqueChara.canShoot)
        {
            attaqueChara.StartCooldown();
            Destroy(gameObject);
        }
    }
}
