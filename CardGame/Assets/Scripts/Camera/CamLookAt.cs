
using UnityEngine;

public class CamLookAt : MonoBehaviour
{
    private void Start()
    {
        CameraSet();
        
    }
    public void CameraSet()
    {
        Vector3 direction = (transform.position - Camera.main.transform.position).normalized;
        direction.x = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
