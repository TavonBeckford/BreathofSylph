using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
	public float radius = 2f;               // How close do we need to be to interact?
	public Transform itemtoInteract;

	public GameObject Player;       // Reference to the player transform

	public bool inRangeForInteraction = false;
	public TMP_Text popupMessage;


	public virtual void Interact()
	{

		Debug.Log("Interacting with " + transform.name);
	}

    private void Start()
    {
		// Find the GameObject with the "player" tag and store it in the 'player' variable.
		Player = GameObject.FindWithTag("Player");
	}

    void Update()
	{

	}

	public void isItemInteractable()
    {
		if (Player != null)
		{
			float distance = Vector3.Distance(Player.transform.position, itemtoInteract.position);
			if (distance <= radius)
			{
				// Interact with the object
				Interact();
				inRangeForInteraction = true;
			}
			else
			{
				inRangeForInteraction = false;
				popupMessage.text = " Too far from item to pick up!";
			}
		}
		else
		{
			// Handle the case when Player is null
			Debug.LogError("Player is not assigned. Make sure the player GameObject is tagged as 'Player'.");
		}
	}
	
	// Draw our radius in the editor
	void OnDrawGizmosSelected()
	{


		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(itemtoInteract.position, radius);
	}

}
