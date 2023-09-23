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

    private void Update()
    {

        rotationX += Input.GetAxis("Mouse Y") * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVertAngle, maxVertAngle);


        rotationY += Input.GetAxis("Mouse X") * rotationSpeed;

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        var focusPosition = followtarget.position + new Vector3(framingOffset.x, framingOffset.y);


        transform.position = focusPosition - targetRotation * new Vector3(0, cameraHeight, cameraDistance);
        transform.rotation = targetRotation;

    }

}
