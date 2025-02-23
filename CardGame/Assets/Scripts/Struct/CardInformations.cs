using Fusion;
using UnityEngine;

[System.Serializable]
public struct CardInformations
{
    public NetworkObject AreaNetworkObj;    // Input authoritynin olduğu 6 lıdan birini tutar.
    public CardData CardData;              // Card Hakkında gerekli herşeyi tutar.
    public PlayerRef player;              // Playerın kim olduğu bilgisini tutar.
    public int CardUniqeID;
}
