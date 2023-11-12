using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{

    [Header("Enemy Attributes & Details")]
    public string enemyName;
    public int hitPoints;
    public int maxHitPoints = 100;
    public int attackPoints;
    public EnemyHealthBar enemyHealthBarSlider;
    public bool enemyHealthBarFirstTimeActive = true;
    public bool inAttackMode= false;
    public bool inPatrolMode = true;

    private bool isHealthRegenerationActive = false;
    public int healthRegenerationRate = 5; 

    private CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        hitPoints = maxHitPoints;
        // Get the CanvasGroup component
        GameObject canGroup = GameObject.FindGameObjectWithTag("EnemyHealthBar");
        canvasGroup = canGroup.GetComponent<CanvasGroup>();

        StartCoroutine(StartHealthRegeneration());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        //Add animation here

        hitPoints -= damage;

        // Check if enemyHealthBarSlider is not null before using it
        if (enemyHealthBarSlider != null)
        {
            enemyHealthBarSlider.SetHealth(hitPoints);
        }
        

        // Check if the enemy's health is zero or less
        if (hitPoints <= 0)
        {
            Die(); // Call the Die method when the health is zero or less
        }

    }

    public IEnumerator DamageOverTime(int damage, int perSecond)
    {
        // Run the loop indefinitely
        while (true)
        {
            // Wait for the specified interval
            yield return new WaitForSeconds(perSecond);

            // Damage the player
            TakeDamage(perSecond);
        }
    }


    private void Die()
    {
        // Perform any death-related actions (e.g., play death animation, disable colliders, etc.)

        // Set the enemy's state to "Dead" in the FSM
        GetComponent<EnemyFSM>().IsDead = true;
    }



    public void SetupHealthBar()
    {
        GameObject enemy = GameObject.FindGameObjectWithTag("EnemyHealthBar");
        enemyHealthBarSlider = enemy.GetComponent<EnemyHealthBar>();
        enemyHealthBarSlider.SetMaxHealthForHealthBar(hitPoints, maxHitPoints);
    }



    IEnumerator StartHealthRegeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(1); // Adjust the interval as needed

            if (isHealthRegenerationActive && inPatrolMode)
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
        if (enemyHealthBarSlider != null)
        {
            enemyHealthBarSlider.SetHealth(hitPoints);
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


    // You can call this method to toggle the visibility of the UI
    public void ToggleVisibilityOnEnemy()
    {
        // Toggle the alpha between 0 and 1
        canvasGroup.alpha = 1;
        canvasGroup.interactable = !canvasGroup.interactable;
        canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;

    }
    public void ToggleVisibilityOffEnemy()
    {
        // Toggle the alpha between 0 and 1
        canvasGroup.alpha = 0;
        canvasGroup.interactable = !canvasGroup.interactable;
        canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
    }

    public void ToggleHealthBarEnemyModeIcon()
    {
        if (inPatrolMode == true)
        {
            GameObject freeRoamIcon = GameObject.FindGameObjectWithTag("PatrolIcon");
            GameObject attackStateIcon = GameObject.FindGameObjectWithTag("AttackIcon");
            Image freeRoamImageofTheIcon = freeRoamIcon.GetComponent<Image>();
            Image attackStateImageofTheIcon = attackStateIcon.GetComponent<Image>();
            attackStateImageofTheIcon.enabled = false;
            freeRoamImageofTheIcon.enabled = true;
        }
        if (inAttackMode == true)
        {
            GameObject freeRoamIcon = GameObject.FindGameObjectWithTag("PatrolIcon");
            GameObject attackStateIcon = GameObject.FindGameObjectWithTag("AttackIcon");
            Image freeRoamImageofTheIcon = freeRoamIcon.GetComponent<Image>();
            Image attackStateImageofTheIcon = attackStateIcon.GetComponent<Image>();
            attackStateImageofTheIcon.enabled = true;
            freeRoamImageofTheIcon.enabled = false;
        }
    }

}
