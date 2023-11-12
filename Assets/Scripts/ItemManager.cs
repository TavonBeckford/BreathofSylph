using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{


    #region Singleton 

    public static ItemManager instance;

    private Items currentItem;
    void Awake()
    {
        instance = this;
    }
    #endregion

    public void UseItem(Items item)
    {
        currentItem = item;
        switch (item.itemType)
        {
            case ItemType.HealthPotion:
                UseHealthPotion();
                break;
            case ItemType.ManaPotion:
                UseManaPotion();
                break;
            case ItemType.Weapon:
                UseWeapon();
                break;
            default:
                Debug.LogWarning("Unsupported item type");
                break;
        }
    }

    private void UseHealthPotion()
    {
        Debug.Log("Using Health Potion");
        // Add specific health potion effects here
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        int newHealth = player.hitPoints + currentItem.value;
        player.UpdateHealth(newHealth);
        Inventory.instance.Remove(currentItem);
    }

    private void UseManaPotion()
    {
        Debug.Log("Using Mana Potion");
        // Add specific mana potion effects here
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        int newMana = player.manaPoints + currentItem.value;
        player.UpdateMana(newMana);
        Inventory.instance.Remove(currentItem);
    }

    private void UseWeapon()
    {
        Debug.Log("Using Weapon");
        // Add specific weapon effects here
        Inventory.instance.Remove(currentItem);
        EquipmentManager.instance.EquipWeapon(currentItem);

    }

}
