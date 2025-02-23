using Fusion;
using TMPro;
using UnityEngine;

public class ManaManager : NetworkBehaviour
{
    public static ManaManager instance;
    [Networked] public  int MaxMana { get; set; }
    [Networked, OnChangedRender(nameof (OnManaChanged))] public int CurrentMana { get; set; }

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else Destroy(instance);
    }

    public override void Spawned()
    {
        MaxMana = GameManager.instance.MaxManaStorage;
        CurrentMana = MaxMana;
    }
    public bool CheckPlayability(int manaCost)
    {
        if (CurrentMana >= manaCost)
        {
            return true;
        }
        else return false;
    }


    public void UseMana (int pickedCardMana)
    {
        
            CurrentMana = CurrentMana - pickedCardMana;
    }

    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void RPC_RefillMana()
    {
        CurrentMana = MaxMana;
    }

   public void OnManaChanged()
    {
        CanvaSys.Instance.ManaText.text = $"{CurrentMana} / {MaxMana}";
    }
}
