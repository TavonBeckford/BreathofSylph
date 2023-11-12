using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform followtarget;
    [SerializeField] float cameraDistance = 5f;
    [SerializeField] float cameraHeight = -1f;

    [SerializeField] float rotationSpeed = 2f;

    [SerializeField] float minVertAngle = -45f;
    [SerializeField] float maxVertAngle = 45f;

    [SerializeField] Vector2 framingOffset;

    float rotationY;
    float rotationX;
    public bool isRotating = true; // Control whether mouse rotation is enabled or not

    public float zoomSpeed = 4f;
    public float minZoom = 0f;
    public float maxZoom = 1.2f;
    public float currentZoom = 0.6f;

    private void Update()
    {
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }



    private void LateUpdate()
    {

        if (isRotating)
        {
            rotationX += Input.GetAxis("Mouse Y") * rotationSpeed;
            rotationX = Mathf.Clamp(rotationX, minVertAngle, maxVertAngle);


            rotationY += Input.GetAxis("Mouse X") * rotationSpeed;
        }
        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        var focusPosition = followtarget.position + new Vector3(framingOffset.x, framingOffset.y);


        transform.position = focusPosition - targetRotation * new Vector3(0, cameraHeight, cameraDistance) * currentZoom;
        transform.rotation = targetRotation;

    }

    //Property for rotation
    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);

}
