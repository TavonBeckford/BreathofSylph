using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOutlineOnStart : MonoBehaviour
{

    private float delay = 0.01f;
    Outline outline;

    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
        Invoke("DisableOutline", delay);
    }

    // Update is called once per frame
    private void DisableOutline()
    {
        outline.enabled = false;
    }


}
