using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    public void SetCameraPosition(Transform CameraTransform)
    {
        Camera.main.transform.position = CameraTransform.position;
        Camera.main.transform.rotation = CameraTransform.rotation;
    }

}
