using UnityEngine;

public class DontDestroyonLoad : MonoBehaviour
{
    public static DontDestroyonLoad instance;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
