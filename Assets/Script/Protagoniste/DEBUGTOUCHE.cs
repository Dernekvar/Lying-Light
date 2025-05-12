using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUGTOUCHE : MonoBehaviour
{
    void Update()
    {
        for (int i = 1; i <= 20; i++) // Essaie jusqu'à l'axe 20
        {
            float value = Input.GetAxisRaw("Axis " + i);
            if (Mathf.Abs(value) > 0.1f)
            {
                Debug.Log("Axis " + i + " = " + value);
            }
        }
    }
}