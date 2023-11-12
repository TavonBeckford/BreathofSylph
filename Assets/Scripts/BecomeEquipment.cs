using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BecomeEquipment : MonoBehaviour
{

    public ItemPickup itemPickup;
    public Interactable interactable;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Check if the EquipmentManager instance is not null
        if (EquipmentManager.instance != null)
        {
            // Check if weaponItem is not null
            if (EquipmentManager.instance.weaponItem != null)
            {

                Debug.Log("I made it this far");
                GameObject equippedWeapon = EquipmentManager.instance.weaponItem.theItem;
                Debug.Log("This is the equipped one"+ equippedWeapon.name);
                Debug.Log("this is the game object" + gameObject.name);
                string originalString = gameObject.name;
                string modifiedString = originalString.Replace("(Clone)", "");
                Debug.Log("this is the modified string" + modifiedString);
                // modifiedString now contains "YourString"

                // Check if the attached GameObject is not null and is the currently equipped weapon
                if (equippedWeapon != null && equippedWeapon.name == modifiedString)
                {
                    // Disable scripts if the attached GameObject is the currently equipped weapon
                    DisableScripts();
                }
                else
                {
                    // Enable scripts if the attached GameObject is not the currently equipped weapon
                    EnableScripts();
                }
            }
        }
    }

    void DisableScripts()
    {
        if (itemPickup != null)
            Destroy(this.GetComponent<ItemPickup>());

        if (interactable != null)
            Destroy(this.GetComponent<Interactable>());
    }

    void EnableScripts()
    {
        if (itemPickup != null)
            itemPickup.enabled = true;

        if (interactable != null)
            interactable.enabled = true;
    }
}
