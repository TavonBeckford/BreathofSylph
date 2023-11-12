using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour
{
	public Image icon;          // Reference to the Icon image
	public Button itemButton;   //Reference to the Use Button or Item Button
	public Button removeButton; // Reference to the remove button
	public TMP_Text stackText;
	public TMP_Text coolDownText;
	public Items tempItem;

	private bool isOnCooldown = false;
	private float cooldownTimer = 0f;

	Items item;  // Current item in the slot

    private void Start()
    {
		coolDownText.enabled = false;
    }

	private void Update()
	{
		// Update cooldown timer if on cooldown
		if (isOnCooldown)
		{
			cooldownTimer -= Time.deltaTime;
			coolDownText.text = Mathf.FloorToInt(cooldownTimer).ToString(); // Round down to nearest integer
																			// Check if cooldown is over
			if (cooldownTimer <= 0f)
			{
				// Cooldown is over
				isOnCooldown = false;

				// Enable interaction
				SetInteractable(true);

				// Hide or update the cooldown text
				coolDownText.text = ""; // You may want to hide the text or set it to an empty string
				coolDownText.enabled = false;
			}
		}
	}





	// Add item to the slot
	public void AddItem(Items newItem)
	{
		item = newItem;

		icon.sprite = item.icon;
		icon.enabled = true;
		stackText.enabled = true;
		stackText.text = (item.stack).ToString();

		removeButton.interactable = true;
	}

	// Clear the slot
	public void ClearSlot()
	{
		tempItem = item;
		item = null;

		icon.sprite = null;
		icon.enabled = false;
		stackText.text = "";
		stackText.enabled = false;
		removeButton.interactable = false;
	}

	// Called when the remove button is pressed
	public void OnRemoveButton()
	{

		if (item.stack == 1)
		{
			stackText.enabled = false;
		}
		Inventory.instance.Remove(item);

	}

	// Called when the item is pressed
	public void UseItem()
	{
		if (item != null)
		{
			// Debugging statements
			Debug.Log("Item stack: " + item.stack);
			Debug.Log("Item type: " + item.itemType);

			// Check and update stack and stackText
			if (item.stack == 1 || item.stack == 0)
			{
				stackText.enabled = false;
				Debug.Log("StackText disabled");

			}
			if (item.itemType == ItemType.Weapon)
            {
				stackText.enabled = false;
            }
			ClearSlot();
			tempItem.Use();

			// Start the cooldown
			StartCooldown(item.cooldown);

			// Disable interaction during cooldown
			SetInteractable(false);




		}
	}


	public Items GetItem()
    {
		return item;
    }


	private void StartCooldown(float cooldownDuration)
	{
		isOnCooldown = true;
		cooldownTimer = cooldownDuration;
		coolDownText.enabled = true;
	}

	private void SetInteractable(bool value)
	{
		// You can set the interactable property of your button here
		itemButton.interactable = value;
	}



}
