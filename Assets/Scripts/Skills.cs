using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : MonoBehaviour
{

    public GameObject firstSkill;
    public GameObject secondSkill;
    public MeteorCircleSkill MeteorCircle => meteorCircle;

    public MeteorCircleSkill meteorCircle { get; private set; }

    public class BaseSkill
    {
        protected string skillName;
        protected string skillDescription;
        protected int damageValue;
        protected float skillDuration;
        protected int manaCost;
        protected int coolDown;

        public BaseSkill(string name, string description, int damage, float duration, int manacost, int cooldown)
        {
            skillName = name;
            skillDescription = description;
            damageValue = damage;
            skillDuration = duration;
            manaCost = manacost;
            coolDown = cooldown;
        }

        public int getDamage()
        {
            return damageValue;
        }

        public int getManaCost()
        {
            return manaCost;
        }

        public int getCoolDown()
        {
            return coolDown;
        }
        public virtual void ActivateSkill(Transform target)
        {
           Debug.Log($"{skillName} activated! {skillDescription} Damage: {damageValue}");
        }
    }

    public class MeteorCircleSkill : BaseSkill
    {

        private GameObject skillPrefab;

        public MeteorCircleSkill(string name, string description, int damage, float duration,int manacost, int cooldown, GameObject skillPrefab) : base(name, description, damage, duration, manacost, cooldown)
        {
            this.skillPrefab = skillPrefab;
            // Additional initialization for MeteorCircleSkill if needed
        }


        public override void ActivateSkill (Transform target)
        {
            // Custom behavior for MeteorCircleSkill
            Debug.Log($"{skillName} casts a fiery explosion! {skillDescription} Damage: {damageValue}");

            GameObject skillObject = Instantiate(skillPrefab, target.position, Quaternion.identity);
            Destroy(skillObject, 5f);

        }
    }

    private void Start()
    {
        // Initializing MeteorCircleSkill
        meteorCircle = new MeteorCircleSkill("Meteor Circle", "Conjours a meteor shower that deals 25 damage per hit. A total of 3 hits", 25, 5f, 50, 20, firstSkill);

       
    }
}
