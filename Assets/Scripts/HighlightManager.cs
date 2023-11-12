using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HighlightManager : MonoBehaviour
{

    private Transform highlightedObj;
    private Transform selectedObj;
    public LayerMask selectablelayer;

    private Outline highlightOutline;
    private RaycastHit hit;




  

    // Update is called once per frame
    void Update()
    {
        HoverHighlight();
    }

    private void HoverHighlight()
    {
        if(highlightedObj != null )
        {
            highlightOutline = highlightedObj.GetComponent<Outline>();
            highlightOutline.enabled = false;
            highlightedObj = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hit, selectablelayer))
        {
            highlightedObj = hit.transform;

            if(highlightedObj.CompareTag("Enemy") && highlightedObj != selectedObj)
            {
                highlightOutline = highlightedObj.GetComponent<Outline>();
                highlightOutline.enabled = true;

            }
            else
            {
                highlightedObj = null;
            }
        }


    }


    public void SelectedHighlight()
    {
        if(highlightedObj.CompareTag("Enemy"))
        {
            if (selectedObj != null)
            {
                selectedObj.GetComponent<Outline>().enabled = false;
            }

            selectedObj = hit.transform;
            selectedObj.GetComponent<Outline>().enabled = true;

            highlightOutline.enabled = true;
            highlightedObj = null;

        }
    }


    public void DeselectHighlight()
    {
        selectedObj.GetComponent<Outline>().enabled = false; 
        selectedObj = null;
    }




    public void TabSelectedHighlight(GameObject gameObj)
    {
        highlightedObj = gameObj.transform;
        if (highlightedObj.CompareTag("Enemy"))
        {
            if (selectedObj != null)
            {
                selectedObj.GetComponent<Outline>().enabled = false;
            }

            selectedObj = highlightedObj;
            selectedObj.GetComponent<Outline>().enabled = true;

            highlightOutline.enabled = true;
            highlightedObj = null;

        }
        highlightedObj = null;

    }


    public bool CurrentlySelected() 
    {
        if (selectedObj != null)
        {
            return true;
        }

        return false;
    }

}
