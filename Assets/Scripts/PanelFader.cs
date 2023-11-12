using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup panelCanvasGroup;
    public float fadeDuration = 0.5f;
    [SerializeField] private bool fadeIn = false;
    [SerializeField] private bool fadeOut= false;
    public void showUI()
    {
        fadeIn = true;
        fadeOut = false;
    }

    public void hideUI()
    {
        fadeOut = true;
        fadeIn = false;
    }

    private void Update()
    {

        if (GameObject.FindGameObjectsWithTag("Item") == null)
        {
            hideUI();
        }


        if (fadeIn)
        {
            if(panelCanvasGroup.alpha < 1)
            {
                panelCanvasGroup.alpha += Time.deltaTime;
                if(panelCanvasGroup.alpha >= 1)
                {
                    fadeIn = false;
                }
            }
        }

        if (fadeOut)
        {
            if (panelCanvasGroup.alpha >= 0)
            {
                panelCanvasGroup.alpha -= Time.deltaTime;
                if (panelCanvasGroup.alpha == 1)
                {
                    fadeOut = false;
                }
            }
        }
    }
}
