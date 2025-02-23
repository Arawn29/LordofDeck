using Fusion;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    public NetworkPrefabRef PlayerPrefab;
    public NetworkPrefabRef CardPlayAreaPrefab;
    public NetworkPrefabRef CardPrefab;
    //public NetworkPrefabRef HealthBar;

    public NetworkPrefabRef ArrowDrawerPrefab;
    public Sprite ArrowToSign;

    public Sprite CardBackFace;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }
}