using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{

    [Header("Player Attributes & Details")]
    public string characterName;
    public int hitPoints;
    public int maxHitPoints = 100;
    public int manaPoints;
    public int maxMana = 100;
    public int attackPoints;
    public int manaProjectileDamage = 2;
    public HealthBar healthBar;
    public ManaBar manaBar;

    private Coroutine damageCoroutine;
    public bool isTakingDamage = false;

    private bool isHealthRegenerationActive = false;
    public int healthRegenerationRate = 5;


    private bool isManaRegenerationActive = true; 
    public int manaRegenerationRate = 2; 

    void Start()
    {
        hitPoints = maxHitPoints;
        manaPoints = maxMana;
        healthBar.SetMaxHealthForHealthBar(maxHitPoints);
        manaBar.SetMaxManaForManaBar(maxMana);

        StartCoroutine(StartHealthRegeneration());
        StartCoroutine(StartManaRegeneration());
    }


    private void Update()
    {
       
    }

    public void UpdateHealth(int health)
    {
        // Ensure that the health does not exceed the maximum value
        hitPoints = Mathf.Clamp(health, 0, maxHitPoints);

        // Update the health bar
        healthBar.SetHealth(hitPoints);
    }

    public void UpdateMana(int manaP)
    {
        // Ensure that the health does not exceed the maximum value
        manaPoints = Mathf.Clamp(manaP, 0, maxMana);

        // Update the health bar
        manaBar.SetMana(manaPoints);
    }
    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        isTakingDamage = true;
        healthBar.SetHealth(hitPoints);

    }

    public void DeductMana(int manaCost)
    {
        manaPoints -= manaCost;
        manaBar.SetMana(manaPoints);
    }

    public IEnumerator DamageOverTime(int damage, int perSecond)
    {
        // Run the loop indefinitely
        while (true)
        {
            // Wait for the specified interval
            yield return new WaitForSeconds(perSecond);

            // Damage the player
            TakeDamage(damage);
        }
    }

    public void StopDamageOverTime()
    {
        // Check if the coroutine is running before attempting to stop it
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
    }
    public void StartDamageOverTime(int dmg, int seconds)
    {
        damageCoroutine = StartCoroutine(DamageOverTime(dmg, seconds));
    }


    IEnumerator StartHealthRegeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(1); // Adjust the interval as needed

            if (isHealthRegenerationActive)
            {
                RegenerateHealth();
            }
        }
    }

    void RegenerateHealth()
    {
        // Regenerate health based on the regeneration rate
        hitPoints += healthRegenerationRate;

        // Clamp the health to the maximum value
        hitPoints = Mathf.Clamp(hitPoints, 0, maxHitPoints);

        // Update the health bar
        if (healthBar != null)
        {
           healthBar.SetHealth(hitPoints);
        }
    }

    // Method to start health regeneration
    public void StartHealthRegeneration(bool startRegeneration)
    {
        isHealthRegenerationActive = startRegeneration;
    }

    // Method to stop health regeneration
    public void StopHealthRegeneration()
    {
        isHealthRegenerationActive = false;
    }


    IEnumerator StartManaRegeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(1); // Adjust the interval as needed

            if (isManaRegenerationActive)
            {
                RegenerateMana();
            }
        }
    }

    void RegenerateMana()
    {
        // Regenerate mana based on the regeneration rate
        manaPoints += manaRegenerationRate;

        // Clamp the mana to the maximum value
        manaPoints = Mathf.Clamp(manaPoints, 0, maxMana);

        // You can perform additional logic here if needed

        // For example, update a mana bar
        if (manaBar != null)
        {
            manaBar.SetMana(manaPoints);
        }
    }

}
