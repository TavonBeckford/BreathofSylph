using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	#region Singleton

	public static Inventory instance;

	void Awake()
	{
		instance = this;
	}

	#endregion

	public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;

	public int space = 20;  // Amount of item spaces

	// Our current list of items in the inventory
	public List<Items> items = new List<Items>();

	// Add a new item if enough room
	public bool Add(Items item)
	{
		if (item)
		{
			if (items.Count >= space)
			{
				Debug.Log("Not enough room.");
				return false;
			}




            if (item.IsStackable())
            {
				bool itemPresentInInventory = false;
				foreach(Items inventoryItem in items)
                {
					if(inventoryItem.itemType == item.itemType)
                    {
						inventoryItem.stack += item.amount;
						itemPresentInInventory = true;
                    }
                }
                if (!itemPresentInInventory)
                {
					items.Add(item);
					item.stack += 1;
				}

//this was added
            }
            else
            {
				items.Add(item);
				item.stack += 1; //If not stackable represent 1 of the item
			}
//to here
			//items.Add(item);

			if (onItemChangedCallback != null)
				onItemChangedCallback.Invoke();
		}
		return true;
	}

	// Remove an item
	public void Remove(Items item)
	{
		//items.Remove(item);


		if (item.IsStackable())
		{

			foreach (Items inventoryItem in items)
			{
				if (inventoryItem.itemType == item.itemType)
				{
					inventoryItem.stack -= item.amount;
					//itemPresentInInventory = true;

				}
			}

			if (item.stack == 0)
			{
				items.Remove(item);
			}

		}
		else
		{
			items.Remove(item);
			item.stack = 0;
			
		}


		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}

	// Reset stack to zero for all items in the inventory when the game scene stops
	private void OnDisable()
	{
		foreach (Items item in items)
		{
			item.stack = 0;
		}
	}


}
