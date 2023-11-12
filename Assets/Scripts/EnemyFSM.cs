using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static Skills;

public class EnemyFSM : MonoBehaviour
{
    [Header("Nav Mesh")]
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    [Header("Timer for States")]
    private float idleTimer;
    float timeBeforeIdle = 5f; // Adjust the time before transitioning to Idle
    public float mintimeBeforeIdle;
    public float maxtimeBeforeIdle;
    private float patrolTimer;
    float timeBeforePatrol = 3f;
    public float mintimeBeforePatrol;
    public float maxtimeBeforePatrol;
    private float hurtTimer;
    public float timeInHurtState = 2f;  // Adjust the time as needed
    private float deathTimer;

    [Header("UI")]
    [Space]


    [Header("Patrolling")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Normal Attack")]
    public float timeBetweenAttacks;
    public float timeBetweenEffects;
    bool alreadyAttacked;
    [Space]
    public GameObject attackPoint;
    public AttackArea attackArea;
    public GameObject normalAttackEffects;
    private bool canNormalAttack = true;
    [Space]
    [Header("Special Attack")]
    public GameObject specialAttackPoint;
    public GameObject specialAttackEffect;
    public float waitToInvokeSpecialAttack;
    public float waitToInvokeSpecialEffects;
    public float specialAttackProbability = 0.5f; // Adjust the probability as needed, 0.2 = 20% chance
    public int specialAttackSecondsPerDamage;
    public float specialAttackDuration;
    private bool isSpecialAttackActive = false;

    [Header("AI Sense")]
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, enemyIsHurt, isTimeToIdle, IsDead;


    [Header("Movement & Animator")]
    private CharacterMovement characterMovement;
    private Animator animator;

    [Header("Animator Parameters")]
    [Space]
    public string walkForward = "Walk Forward";
    public string runForward = "Run Forward";
    public string stabAttack = "Stab Attack";
    public string hurt = "Take Damage";
    public string die = "Die";

    [Space]
    [Header("Parameters to handle chasing after a projectile hit and taking damage")]
    bool isAttackedByPlayer = false;
    // Add a boolean variable to track if any enemy is in combat
    private bool anyEnemyInCombat = false;
    private GameObject hurtTrigger; // Reference to the trigger GameObject


    private bool isInHurtTrigger = false;

    public Enemy enemy;


    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack,
        Hurt,
        Idle,
        Dead
    }

    public EnemyState currentState = EnemyState.Patrol;


    private void Awake()
    {
        player = GameObject.Find("CROISSANT").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemy = this.GetComponent<Enemy>();

    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            characterMovement = player.GetComponent<CharacterMovement>();
        }
        else
        {
            Debug.LogError("OtherObject not found.");
        }

        // Initialize timeBeforeIdle with a random value between min and max
        timeBeforeIdle = UnityEngine.Random.Range(mintimeBeforeIdle, maxtimeBeforeIdle);

        // Initialize timeBeforePatrol with a random value between min and max
        timeBeforePatrol = UnityEngine.Random.Range(mintimeBeforePatrol, maxtimeBeforePatrol);

    }

    void Update()
    {
        FSM();

        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);


        if (isAttackedByPlayer==false && !playerInSightRange && !playerInAttackRange && !enemyIsHurt && !isTimeToIdle && !IsDead)
        {
            ChangeState(EnemyState.Patrol);
            enemy.inPatrolMode = true;
            enemy.inAttackMode = false;
            enemy.StartHealthRegeneration(true);
            anyEnemyInCombat = false;

            // Check if any enemy is still in combat before setting it to false
            if (IsAnyEnemyInCombat() == false)
            {

                characterMovement.inCombat = false;
            }

        }

        if (isAttackedByPlayer == true || playerInSightRange && !playerInAttackRange && !enemyIsHurt && !isTimeToIdle && !IsDead)
        {
            ChangeState(EnemyState.Chase);
            
            enemy.inPatrolMode = false;
            enemy.inAttackMode = true;
            enemy.StopHealthRegeneration();
            
            // Set the boolean variable to true to indicate that at least one enemy is in combat
            anyEnemyInCombat = true;
            characterMovement.inCombat = true;


        }

        if (playerInAttackRange && playerInSightRange && !enemyIsHurt && !isTimeToIdle && !IsDead)
        {
            ChangeState(EnemyState.Attack);
            enemy.inPatrolMode = false;
            enemy.inAttackMode = true;
            // Set the boolean variable to true to indicate that at least one enemy is in combat
            anyEnemyInCombat = true;
            characterMovement.inCombat = true;

        }

        if(enemyIsHurt == true)
        {
            ChangeState(EnemyState.Hurt);
        }

        if (isTimeToIdle ==true)
        {

            ChangeState(EnemyState.Idle);
            idleTimer = 0f; // Reset the timer
        }

        if (IsDead == true)
        {
            ChangeState(EnemyState.Dead);
        }

        // Check if the hurtTrigger GameObject has been destroyed or disabled
        if (hurtTrigger == null)
        {
            isInHurtTrigger = false;
            enemyIsHurt = false;
        }
    }


    private void FSM()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
               Patroling();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                break;
            case EnemyState.Attack:
                AttackPlayer();
                break;
            case EnemyState.Hurt:
                EnemyHurt();
                break;
            case EnemyState.Idle:
                Idling();
                break;
            case EnemyState.Dead:
                Dead();
                break;
            default:
                //throw new Exception("current State is invalid");
                print("current State is invalid" + currentState);
                break;

        }
    }

    private void Idling()
    {
        animator.SetBool(walkForward, false);
        patrolTimer += Time.deltaTime;

        if (patrolTimer >= timeBeforePatrol)
        {
            //ChangeState(EnemyState.Patrol);
            isTimeToIdle = false;
            patrolTimer = 0f; // Reset the timer
            return;
        }

    }

    private void AttackPlayer()
    {
        //Stop enemy from moving
        agent.SetDestination(transform.position);
        animator.SetBool(runForward, false);
        

        transform.LookAt(player);


        if (!alreadyAttacked)
        {

            //Attack Code and Animations here
            animator.SetTrigger(stabAttack);

            if (attackArea.isPlayerInZone == true)
            { 
               Invoke(nameof(NormalAttacks), timeBetweenEffects);
               Invoke(nameof(InstantiateEffectNormalAttack), timeBetweenEffects);

            }


            /***********************************************COMPLETE SPECIAL AND NORMAL ATTACK FOR BOSS MOBS*************************
            if (!alreadyAttacked && canNormalAttack)
            {

                //Attack Code and Animations here
                animator.SetBool(stabAttack, true);

                if (attackArea.isPlayerInZone == true)
                {


                    int numberOfAttacks = UnityEngine.Random.Range(2, 6); // Generates a random number between 2 (inclusive) and 6 (exclusive)

                    for (int i = 0; i < numberOfAttacks; i++)
                    {
                        //Invoke(nameof(NormalAttacks), timeBetweenEffects);
                        //Invoke(nameof(InstantiateEffectNormalAttack), timeBetweenEffects);
                    }
                    // Check if a special attack should be triggered
                    bool triggerSpecialAttack = UnityEngine.Random.Range(0f, 1f) < specialAttackProbability;

                    if (triggerSpecialAttack && !isSpecialAttackActive)
                    {
                        animator.SetBool(stabAttack, false);
                        canNormalAttack = false; // Disable normal attacks during special attack
                        isSpecialAttackActive = true; // Set the flag to indicate a special attack is active
                        SpecialAttack();
                        Invoke(nameof(SetSpecialAttackOnPlayer), waitToInvokeSpecialAttack);
                        Invoke(nameof(InstantiateEffectSpecialAttack), waitToInvokeSpecialEffects);
                        //Invoke(nameof(SetSpecialAttackOnPlayer), waitToInvokeSpecialAttack);
                        Invoke(nameof(StopSpecialAttack), specialAttackDuration + 1); // I added 1 to the duration because it was late by a second for some reason so the damage when duration was 10 seconds and the seconds per dmg was 2 was 40.
                        Invoke(nameof(EnableNormalAttack), specialAttackDuration + 1); // Enable normal attacks after special attack duration
                    }


                }***********************************************************************************************************/

            //Invoke(nameof(InstantiateEffectNormalAttack), timeBetweenEffects);

            //

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

    }

    public void NormalAttacks()
    {
        Player playerToAttack = player.GetComponent<Player>();
        playerToAttack.TakeDamage(2);
    }

    private void EnableNormalAttack()
    {
        canNormalAttack = true;
    }

    void InstantiateEffectNormalAttack()
    {
        Instantiate(normalAttackEffects, attackPoint.transform.position, Quaternion.identity);
    }

    public void SpecialAttack()
    {
        Player playerToAttack = player.GetComponent<Player>();
        playerToAttack.StartDamageOverTime(this.GetComponent<Enemy>().attackPoints, specialAttackSecondsPerDamage);
    }

    public void StopSpecialAttack()
    {
        if (isSpecialAttackActive)
        {
            Player playerToAttack = player.GetComponent<Player>();
            playerToAttack.StopDamageOverTime();
            isSpecialAttackActive = false; // Reset the special attack flag
        }
        specialAttackPoint.SetActive(false);
        attackPoint.SetActive(true);
    }

    void SetSpecialAttackOnPlayer()
    {
        attackPoint.SetActive(false);
        specialAttackPoint.SetActive(true);
        specialAttackPoint.transform.position = player.transform.position;

    }

    void DisableSpecialAttackGameObject()
    {
        // Destroy the GameObject
        specialAttackPoint.SetActive(false);
    }
    void InstantiateEffectSpecialAttack()
    {
        GameObject vfx;
        vfx =Instantiate(specialAttackEffect, player.transform.position, Quaternion.identity);
        vfx.GetComponent<AutoDestroy>().delay = specialAttackDuration;
        
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
        animator.SetBool(stabAttack, false);
    }



    private bool IsAnyEnemyInCombat()
    {
        // This assumes you have a list of EnemyFSM scripts on each enemy GameObject
        EnemyFSM[] enemies = GameObject.FindObjectsOfType<EnemyFSM>();

        foreach (var enemy in enemies)
        {
            if (enemy.anyEnemyInCombat == true)
            {
                return true;
            }
        }

        return false;
    }



    private void ChasePlayer()
    {
        animator.SetBool(walkForward, false);
        animator.SetBool(runForward, true);
        agent.SetDestination(player.position);
        isAttackedByPlayer = false;


    }

    private void Patroling()
    {
        animator.SetBool(walkForward, true);
        //freeRoamIcon.gameObject.SetActive(true);
        //attackStateIcon.gameObject.SetActive(false);


        idleTimer += Time.deltaTime;

        if (idleTimer >= timeBeforeIdle)
        {
            isTimeToIdle = true;
            return;
        }

        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;


        //Walkpoint Reached
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }


    private void EnemyHurt()
    {
        animator.SetBool(walkForward, false);
        hurtTimer += Time.deltaTime;

        if(IsDead == true)
        {
            isInHurtTrigger = false;
            enemyIsHurt = false;
        }

        if (hurtTimer >= timeInHurtState && !isInHurtTrigger)
        {
            enemyIsHurt = false;
            hurtTimer = 0f;  // Reset the timer
        }

        animator.SetTrigger(hurt);
    }


    private void Dead()
    {
        isInHurtTrigger = false;
        enemyIsHurt = false;
        animator.SetBool(walkForward, false);
        animator.SetBool(runForward, false);
        animator.SetBool(stabAttack, false);
        animator.SetBool(hurt, false);
        animator.SetTrigger(die);
        deathTimer += Time.deltaTime;

        if (deathTimer >= 5)
        {
            characterMovement.inCombat = false;
            TargetLock targetLock = player.GetComponent<TargetLock>();
            targetLock.EnemyDeathDeselect();
        }

        // Additional logic for instantiating dropped item and destroying the GameObject
        if (deathTimer >= 5)
        {
            InstantiateDroppedItem();
            Destroy(gameObject);
        }
    }


    private void InstantiateDroppedItem()
    {
        // Find the DroppedItem object in the scene
        GameObject droppedItemObject = GameObject.Find("Item Manager");

        // Check if the DroppedItem object exists in the scene
        if (droppedItemObject != null)
        {
            DroppedItem droppedItem = droppedItemObject.GetComponent<DroppedItem>();

            // Check if the DroppedItem script is attached to the object
            if (droppedItem != null)
            {
                GameObject itemtodrop = droppedItem.DropItem();
                // Adjust the Y position
                Vector3 newPosition = transform.position;
                newPosition.y -= 3f; 
                itemtodrop.transform.position = transform.position;
            }
            else
            {
                Debug.LogError("DroppedItem script not found on the DroppedItem object.");
            }
        }
        else
        {
            Debug.LogError("DroppedItem object not found in the scene.");
        }

        // Destroy the current enemy GameObject
        Destroy(gameObject);
    }


    private void SearchWalkPoint()
    {
        //Calculate a random point in range
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }

    }

    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        Debug.Log("State Changes to:" + currentState);
    }


    /// Take Projectile Damage
    private void OnCollisionEnter(Collision collision)
    {


        if (collision.gameObject.tag == "Projectile")
        {
            enemy.TakeDamage(player.GetComponent<Player>().manaProjectileDamage);
            if (isInHurtTrigger == false)  // Only set enemyIsHurt if not already in the trigger
            {
                enemyIsHurt = true;

                if (currentState == EnemyState.Patrol)
                {
                    isAttackedByPlayer = true;
                    ChangeState(EnemyState.Chase);
                }
                
                //ChangeState(EnemyState.Chase);
            }

            //enemy.TakeDamage(player.GetComponent<Player>().manaProjectileDamage);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("First Skill"))
        {
            Skills skill = GameObject.FindGameObjectWithTag("Player").GetComponent<Skills>();
            MeteorCircleSkill meteorCircleInstance = skill.meteorCircle;
            hurtTrigger = other.gameObject;
            isInHurtTrigger = true;
            enemyIsHurt = true;
            enemy.TakeDamage(meteorCircleInstance.getDamage());
          
        }
    }

  



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

}
