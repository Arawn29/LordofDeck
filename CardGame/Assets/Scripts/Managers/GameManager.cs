using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;



public class GameManager : NetworkBehaviour
{

    public static GameManager instance;

    public Dictionary<PlayerRef, List<NetworkObject>> PlayersCards = new Dictionary<PlayerRef, List<NetworkObject>>();
    [Header("ManaConfig")]
    public int MaxManaStorage;
    [Header("HealthConfig")]
    public int healthStorage;
    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(instance); }


    }
    private void Start()
    {
        PlayersCards.Clear();
    }

    #region Try&PlayCardUI
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_CheckCardStateability(bool isFrontly, PlayerRef player, int cardID, NetworkObject hittedObject, int CardUniqueID)
    {
        StartCoroutine(CheckStateabilityCoroutine(isFrontly, player, cardID, hittedObject, CardUniqueID));
    }
    IEnumerator CheckStateabilityCoroutine(bool isFrontly, PlayerRef player, int cardID, NetworkObject hittedObject, int CardUniqueID)
    {
        NetworkObject AreasNetworkObj = hittedObject;

        CardData cardData = FindCardFromCardDataMagazine(cardID);


        (bool, NetworkObject) Check = CheckTheField(cardData.CardType, AreasNetworkObj);

        if (Check.Item1 && Check.Item2 != null)
        {
            NetworkObject createdCard = CardManager.Instance.CreateCard(Check.Item2, player);

            yield return new WaitUntil(() => createdCard != null && createdCard.IsValid);  
            if (createdCard != null)
            {
                Card card = createdCard.GetComponent<Card>();
                card.Inýtializer(cardData, player, Check.Item2, isFrontly);
                SoundManager.Instance.RPC_PlaySound("CardUIPlaySound");
                RPC_CardUIDestroy(player, CardUniqueID);
                ManaManager.instance.UseMana(cardData.ManaCost);
                RPC_AddCardforPlayerList(player, createdCard);
            }
        }
    }
    (bool, NetworkObject) CheckTheField(CardType cardType, NetworkObject AreasNetworkObj)
    {
        CardArea[] cardAreaScripts = AreasNetworkObj.gameObject.GetComponentsInChildren<CardArea>();
        int intCardType = cardType == CardType.AttackCard ? 0 : 1;

        foreach (CardArea cardArea in cardAreaScripts)
        {
            if ((int)cardArea.CardareaType == intCardType && cardArea.isEmpty == true)
            {

                cardArea.isEmpty = false;
                NetworkObject Area = cardArea.GetComponent<NetworkObject>(); // 6 lýdan müsait olan alan 
                return (true, Area);
            }
        }

        return (false, null);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_CardUIDestroy(PlayerRef player, int cardUniqueID)
    {

        if (Runner.LocalPlayer == player)
        {

            for (int i = 0; i < CardInventory.Instance.CardsOnHand.Count; i++)
            {
                GameObject cardData = CardInventory.Instance.CardsOnHand[i];
                if (cardData.GetComponent<CardUI>().CardUniqueID == cardUniqueID)
                {

                    cardData.GetComponent<CardUI>().CardPlayed();
                    break;
                }
            }
           CanvaSys.Instance.InventoryMove();
        }

    }


    #endregion

    public void GetActivePlayersInGame()
    {
        foreach (PlayerRef player in Runner.ActivePlayers)
        {
            Debug.Log(player);
            MultiplayerSpawner.Instance.PlayersInScene.Add(player);

        }
    }
    public CardData FindCardFromCardDataMagazine(int cardID)
    {
        foreach (CardData card in CardMagazine.instance.AllCards)
        {
            if (card.CardID == cardID)
            {
                return card;
            }
        }
        return null;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AddCardforPlayerList(PlayerRef player, NetworkObject Card)
    {
        if (PlayersCards.TryGetValue(player, out List<NetworkObject> PlayerCards))
        {
            PlayerCards.Add(Card);
            Debug.LogWarning("KartEklendi");
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_RemoveCardforPlayerList(PlayerRef player, NetworkObject Card)
    {
        if (PlayersCards.TryGetValue(player, out List<NetworkObject> cardlist))
        {

            cardlist?.Remove(Card);
        }
    }
    public List<Card> GetCardsofPlayer(PlayerRef player)
    {
        List<Card> cards = new List<Card>();
        if (PlayersCards.TryGetValue(player, out List<NetworkObject> playerCards))
        {
            foreach (var card in playerCards)
            {

                Card _card = card.GetComponent<Card>();
                if (_card != null)
                {
                    cards.Add(_card);
                }
            }
        }
        return cards;

    }
    public Card GetACardofPlayer(PlayerRef player, CardData cardData)
    {
        List<Card> playersCards = GetCardsofPlayer(player);
        Card theCard = null;
        foreach (var item in playersCards)
        {
            if (item._CardData == cardData)
            {
                theCard = item;
            }
        }
        return theCard;
    }
    public List<Card> GetSpecificCardsofPlayer(CardType cardType, PlayerRef player)
    {
        List<Card> cards = GetCardsofPlayer(player);
        List<Card> filteredCards = new List<Card>();
        foreach (var item in cards)
        {
            if (item._CardData.CardType == cardType)
            {
                filteredCards.Add(item);
            }
        }

        return filteredCards;
    }
    public List<Card> GetAllCardsOnScene()
    {
        List<Card> allCards = new List<Card>();
        foreach (var value in PlayersCards.Values)
        {
            for (int i = 0; i < value.Count; i++)
            {
                allCards.Add(value[i].GetComponent<Card>());
            }

        }
        return allCards;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_CardListCreating()
    {
        foreach (var _player in Runner.ActivePlayers)
        {
            if (!PlayersCards.ContainsKey(_player))
            {
                PlayersCards.Add(_player, new List<NetworkObject>());
                Debug.Log($"{_player} için bir Liste  bu fonksiyon sadece clientte çalýþýr.");
            }
        }
    }
    //public void PlayerLeft(PlayerRef player)
    //{
    //    if (PlayersCards.ContainsKey(player))
    //    {

    //        PlayersCards.Remove(player);

    //        Debug.Log($"{player} oyundan ayrýldý liste kaldýrýlýyor.)");

    //    }
    //}

    public NetworkObject GetPlayerAreas(PlayerRef player)
    {
        NetworkObject areas = Runner.GetPlayerObject(player);
        return areas;
    }
    public bool IsItServer()
    {
        if (Runner.IsServer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GameWinner()
    {
        GameObject panel = CanvaSys.Instance.WinLosePanel;
        panel.SetActive(true);
        panel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Victory";
        SoundManager.Instance.PlaySound("Won");
        //LobbyManager.Instance.ShutDown();
        //StartCoroutine(Shutdown());
    }
    public void GameLoser()
    {
        GameObject panel = CanvaSys.Instance.WinLosePanel;
        panel.SetActive(true);
        panel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "Defeated";
        SoundManager.Instance.PlaySound("Defeated");
       
        //StartCoroutine(Shutdown());
        

    }
    //IEnumerator  Shutdown()
    //{
    //    LobbyManager.Instance.ShutDown();
    //}

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ChooseWinner(PlayerRef player)
    {
        // player = loser
        if (player == Runner.LocalPlayer)
        {
            GameLoser();
        }
        else
        {
            GameWinner();
        }
    }
}




