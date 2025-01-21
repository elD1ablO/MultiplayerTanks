using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour
{
    public Camera targetCamera; // Assign the camera in the inspector or via script

    void Start()
    {        
        if (targetCamera == null)
        {
            targetCamera = Helpers.Camera;
        }
    }

    void Update()
    {
        // Make the canvas face the camera
        if (targetCamera != null)
        {
            transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                             targetCamera.transform.rotation * Vector3.up);
        }
    }
}

