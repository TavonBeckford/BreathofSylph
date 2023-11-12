using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Health UI")]
    public Slider healthSlider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxHealthForHealthBar(int health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;

        fill.color = gradient.Evaluate(1f);
    }


    public void SetHealth(int health)
    {
        healthSlider.value = health;


        fill.color = gradient.Evaluate(healthSlider.normalizedValue);

    }


}
