using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = " New Item", menuName ="Item/Create New Item")]


public class Items : ScriptableObject
{
    public int id;
    public string itemName;
    public int value;
    public Sprite icon;
    public int amount = 1;
    public int stack = 1;
    public string description;
    public ItemType itemType;
    public float cooldown;
    public GameObject theItem;
    



    public bool IsStackable()
    {

        return itemType == ItemType.HealthPotion || itemType == ItemType.ManaPotion;
    }


   

    public void Use()
    {
        ItemManager.instance.UseItem(this);
    }


}

public enum ItemType
{
    ManaPotion,
    HealthPotion,
    Weapon,
    Helmet,
    Armor,
    LeftGlove,
    RightGlove,
    Boots
}

