using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CineCamController : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;

    public float zoomSpeed = 10.0f;
    public float minFOV = 30.0f;
    public float maxFOV = 60.0f;

    private void Update()
    {
        // Get the scroll wheel input value
        float scrollInput = -Input.GetAxis("Mouse ScrollWheel");

        // Calculate the new Field of View (FOV)
        float newFOV = freeLookCamera.m_Lens.FieldOfView + (scrollInput * zoomSpeed);

        // Clamp the FOV within the minFOV and maxFOV range
        newFOV = Mathf.Clamp(newFOV, minFOV, maxFOV);

        // Update the Cinemachine FreeLook camera's Field of View
        freeLookCamera.m_Lens.FieldOfView = newFOV;
    }
}
