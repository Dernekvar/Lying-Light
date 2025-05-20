using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    private AttaqueChara attaqueChara;

    private void Start()
    {
        attaqueChara = FindObjectOfType<AttaqueChara>();
    }

    private void Update()
    {
       
           if (attaqueChara.isCharging)
            {
                gameObject.tag = "Charge";
            }

           else if (attaqueChara.canShoot)
            {
                gameObject.tag = "PlayerProjectile";
            }
        
    }
}