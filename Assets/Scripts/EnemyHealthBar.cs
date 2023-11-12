using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health UI")]
    public Slider healthSlider;
    public Image fill;

    public void SetMaxHealthForHealthBar(int health, int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;


    }


    public void SetHealth(int health)
    {
        healthSlider.value = health;


    }
}
