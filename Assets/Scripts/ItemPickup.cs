using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Items Item;
    public Interactable interactable;
    public float destroyDelay = 60.0f; // 60 seconds 
    public PanelFader panelFader;
    public float messageDisplayTime = 5f;

    private void Start()
    {
        Interactable interactable = this.GetComponent<Interactable>();
        GameObject panel = GameObject.FindGameObjectWithTag("PopupMessage");
        panelFader = panel.GetComponent<PanelFader>();
        StartCoroutine(DestroyAfterDelay(destroyDelay));
    }

    void Pickup()
    {
        interactable.isItemInteractable();
        //InventoryManager.Instance.Add(Item);
        if (interactable.inRangeForInteraction == true)
        {
            bool wasPickedUp = Inventory.instance.Add(Item);

            if (wasPickedUp)
                Destroy(gameObject);
                Invoke(nameof(ClosePopUpMessage), messageDisplayTime);
        }
        else
        {
            panelFader.showUI();
            Invoke(nameof(ClosePopUpMessage), messageDisplayTime);
        }
    }


    private void OnMouseDown()
    {
        if (isActiveAndEnabled)
        {
            Pickup();
        }

    }


    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // After the delay, destroy the GameObject
        Destroy(gameObject);
    }

    private void ClosePopUpMessage()
    {
        panelFader.hideUI();
    }

}
