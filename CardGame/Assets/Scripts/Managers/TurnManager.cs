using Fusion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    public static TurnManager instance;

    [Networked, OnChangedRender(nameof(OnPlayerChanged))]
    public PlayerRef TurnOwner { get; set; } // Only server can set
    public static bool IsTurnOwner { get; set; }
    public static event Action<PlayerRef> OnRoundChanged;

    int CurrentPlayerIndex = 0;
    GameObject TurnInfoPan;
    PlayerRef firstPlayer;

    [Networked,OnChangedRender(nameof(OnRoundCountChanged))] public int RoundCount { get; set; }

    private void OnRoundCountChanged()
    {
       TextMeshProUGUI text =  CanvaSys.Instance.RoundPan.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "Round " + RoundCount.ToString();
    }


    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
       
        TurnInfoPan = CanvaSys.Instance.TurnInfoPan;
        TurnInfoPan.SetActive(false);
    }
    //-------SERVER-----
    public void Initializer()
    {
        RoundCount = 1;
        TurnOwner = MultiplayerSpawner.Instance.PlayersInScene[CurrentPlayerIndex];
        firstPlayer = TurnOwner;
    }

    #region Buton&RPC
    public void EndTurnButton()
    {
        RPC_ChangePlayerTurn(Runner.LocalPlayer);
    }



    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ChangePlayerTurn(PlayerRef sender)
    {
        if (CurrentTurnOwner() == sender)
        {
            if (firstPlayer != sender)
            {
               ++RoundCount;
            }
            
            CurrentPlayerIndex++;

            if (CurrentPlayerIndex >= MultiplayerSpawner.Instance.PlayersInScene.Count)
            {
                CurrentPlayerIndex = 0;
            }

            PlayerRef player = MultiplayerSpawner.Instance.PlayersInScene[CurrentPlayerIndex];

            TurnOwner = player;
            ManaManager.instance.RPC_RefillMana();
            SoundManager.Instance.RPC_PlaySound("TurnChange");

        }
    }
    #endregion

    public  void OnPlayerChanged()
    {
        Debug.Log($"OnRoundChanged Subscribers Count: {TurnManager.OnRoundChanged?.GetInvocationList().Length ?? 0}");
        if (Runner.LocalPlayer == TurnOwner)
        {
            RPC_ResetCardStates();
            IsTurnOwner = true;
            InfoPanSet("Your", Color.green);
            Deck.isButtonPressed = false;


        }
        else
        {
            IsTurnOwner = false;
            InfoPanSet("Enemy", Color.red);
            Deck.isButtonPressed = true;
        }

        Invoke(nameof(RoundChangeEvent), 0.2f);
    }
    public void RoundChangeEvent()
    {
        OnRoundChanged?.Invoke(TurnOwner);
    }
   
    public PlayerRef CurrentTurnOwner() { return TurnOwner; }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ResetCardStates()
    {

        List<Card> playerCards = GameManager.instance?.GetCardsofPlayer(TurnOwner);
        if (playerCards.Count > 0)
        {
            foreach (Card card in playerCards)
            {
                card.isCardHasPlayed = false;
            }
        }

    }
    public void InfoPanSet(string Player, Color color)
    {
        TurnInfoPan.SetActive(true);
        TextMeshProUGUI PlayerText = TurnInfoPan.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        PlayerText.text = $"{Player} Turn";
        PlayerText.color = color;
    }
}


