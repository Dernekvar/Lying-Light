using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth; // Référence au script PlayerHealth
    public Image[] hearts; // Tableau d'images de cœurs dans l'UI

    void Start()
    {
        UpdateHearts();
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < playerHealth.currentHealth)
            {
                hearts[i].enabled = true; // Active le cœur
            }
            else
            {
                hearts[i].enabled = false; // Désactive le cœur
            }
        }
    }
}
