using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Skills;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Animator animator;

    private TargetLock targetLock;
    [SerializeField] public SpawnProjectile spawnProjectile;

    public float maxDistance = 10f;

    public bool isAttackingMonster = false;

    [Header("Layer Index For Animation")]
    private int baseLayerIndex;
    private int combatLayerIndex;
    private int basicMagicAttackLayerIndex;
    private Player player;


    [Header("Animator Parameters")]
    [Space]
    public string walkForward = "Walk Forward";
    public string runForward = "Run Forward";
    public string stabAttack = "Stab Attack";
    public string hurt = "Take Damage";

    [Header("Skill")]
    private Skills skill;
    private bool isFirstSkillOnCooldown = false;

    [Header("Pop Up Message Section")]
    public TMP_Text popupMessage;
    public PanelFader panelFader;
    public float messageDisplayTime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        targetLock = GetComponent<TargetLock>();
        player = GetComponent<Player>();
        basicMagicAttackLayerIndex = animator.GetLayerIndex("Basic Magic Attack Layer");
        skill = FindObjectOfType<Skills>();

    }

    // Update is called once per frame
    void Update()
    {
        spawnProjectile.IsFirePointActiveAndEquipped();
        // Check if the right mouse button is clickedj
        if (Input.GetMouseButtonDown(1) && targetLock.isTargeting == true && EquipmentManager.instance.playercanfire == true)  // 1 = Right Mouse Click
        {
            // Check the distance condition (e.g., if closer than 10 units)
            if (Vector3.Distance(transform.position, targetLock.TheCurrentEnemy().transform.position) < maxDistance)
            {
                spawnProjectile.enabled = true;
                animator.SetLayerWeight(basicMagicAttackLayerIndex, 1);
                Invoke(nameof(ClosePopUpMessage), messageDisplayTime);

            }
            else
            {
                popupMessage.text = "Too far from the enemy to attack!";
                panelFader.showUI();
                Invoke(nameof(ClosePopUpMessage), messageDisplayTime);
            }


        }


        if(targetLock.isTargeting == false)
        {
            spawnProjectile.enabled = false;
            animator.SetLayerWeight(basicMagicAttackLayerIndex, 0);

        }

        if(player.isTakingDamage == true)
        {
            spawnProjectile.enabled = false;
            animator.SetLayerWeight(basicMagicAttackLayerIndex, 0);
            animator.SetTrigger(hurt);
            spawnProjectile.enabled = true;
            animator.SetLayerWeight(basicMagicAttackLayerIndex, 1);
            player.isTakingDamage = false;

        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(EquipmentManager.instance.weaponItem != null)
            {
                useFirstSkill();
                Invoke(nameof(ClosePopUpMessage), messageDisplayTime);
            }


            if (EquipmentManager.instance.weaponItem == null) { 

                popupMessage.text = "No weapon equipped to use skill";
                panelFader.showUI();
                Invoke(nameof(ClosePopUpMessage), messageDisplayTime);
            }
            
            
        }



    }


    public void useFirstSkill()
    {
        if (!isFirstSkillOnCooldown)
        {
            // Access the meteorCircle instance
            MeteorCircleSkill meteorCircleInstance = skill.meteorCircle;

            // Activate the skill
            meteorCircleInstance.ActivateSkill(targetLock.TheCurrentEnemy().transform);

            // Deduct mana
            player.DeductMana(meteorCircleInstance.getManaCost());

            // Start cooldown
            StartCoroutine(StartFirstSkillCooldown(meteorCircleInstance.getCoolDown()));
            Invoke(nameof(ClosePopUpMessage), messageDisplayTime);
        }
        else
        {
            // Skill is on cooldown, you can add some feedback or just ignore the button press
            popupMessage.text = "Meteor Circle is on cooldown";
            panelFader.showUI();
            Invoke(nameof(ClosePopUpMessage), messageDisplayTime);
        }
    }

    IEnumerator StartFirstSkillCooldown(int coolDownTime )
    {
        isFirstSkillOnCooldown = true;

        // Wait for the cooldown time
        yield return new WaitForSeconds(coolDownTime);

        // Cooldown is over
        isFirstSkillOnCooldown = false;
    }

    public void useSecondSkill()
    {

    }


    public void isPicking()
    {
        animator.SetTrigger("IsPicking");
    }
    private void ClosePopUpMessage()
    {
        panelFader.hideUI();
    }


}
