using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
	public Transform itemsParent;   // The parent object of all the items
	public GameObject inventoryUI;  // The entire UI

	Inventory inventory;    // Our current inventory

	InventorySlot[] slots;  // List of all the slots

	[SerializeField] CinemachineFreeLook freeLookCamera;   //grab camera object

	public bool isInventoryOpen= false;


	void Start()
	{
		inventory = Inventory.instance;
		inventory.onItemChangedCallback += UpdateUI;    // Subscribe to the onItemChanged callback

		// Populate our slots array
		slots = itemsParent.GetComponentsInChildren<InventorySlot>();
	}

	void Update()
	{
		// Check to see if we should open/close the inventory
		if (Input.GetKeyDown(KeyCode.I))
		{

			if (isInventoryOpen)
			{

				freeLookCamera.m_XAxis.m_MaxSpeed = 450;
				freeLookCamera.m_YAxis.m_MaxSpeed = 4;
				inventoryUI.SetActive(false);
				isInventoryOpen = false;
			}
			else
			{
				freeLookCamera.m_XAxis.m_MaxSpeed = 0;
				freeLookCamera.m_YAxis.m_MaxSpeed = 0;
				inventoryUI.SetActive(true);
				isInventoryOpen = true;
			}
		}

	}

	// Update the inventory UI by:
	//		- Adding items
	//		- Clearing empty slots
	// This is called using a delegate on the Inventory.
	public void UpdateUI()
	{
		// Loop through all the slots
		for (int i = 0; i < slots.Length; i++)
		{
			if (i < inventory.items.Count)  // If there is an item to add
			{
				slots[i].AddItem(inventory.items[i]);   // Add it
			}
			else
			{
				// Otherwise clear the slot
				slots[i].ClearSlot();
			}
		}
		// Check for null slots and clear them
		ClearNullSlots();
	}


	// Clear null slots
	private void ClearNullSlots()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].GetItem() == null)
			{
				slots[i].ClearSlot();
			}
		}
	}
}
