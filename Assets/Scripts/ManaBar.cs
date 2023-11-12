using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [Header("Mana UI")]
 
    public Slider manaSlider;


    public void SetMaxManaForManaBar(int mana)
    {
        manaSlider.maxValue = mana;
        manaSlider.value = mana;
    }

    
    public void SetMana(int mana)
    {
        manaSlider.value = mana;
    }
}
