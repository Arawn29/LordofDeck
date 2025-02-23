using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvaSys : NetworkBehaviour
{
    public static CanvaSys Instance;

    MoveManager _Cardsmovemanager;

    public GameObject InventoryCardParent;
    public GameObject MiddleOfScene;
    public GameObject HandPosition;




    [Header("AligngParameters")]

    [Tooltip("MaxAngle 180 den büyük deðer olmamalý")]
    [Range(0f, 160)]
    public float MaxAngle = 120f;

    public float radius = 200f;

    [SerializeField] private float CardMoveDuration;

    [Header("CardFaceSettings")]
    public GameObject SetFaceOfCardPanel;
    [Header("Mana Canvas")]
    public TextMeshProUGUI ManaText;
    [Header("EndTurnPan")]
    public GameObject TurnButton;
    public GameObject TurnInfoPan;

    [Header("InfoPan")]
    public GameObject InfoPan;
    [Header("RoundPan")]
    public GameObject RoundPan;
    [Header("Win/LosePan")]
    public GameObject WinLosePanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(Instance);
        }
        _Cardsmovemanager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MoveManager>();
        InfoPan.SetActive(false);
        WinLosePanel.SetActive(false);
    }
    public void InventoryMove()
    {
        StartCoroutine(InventoryMoveCoroutine());
    }
    public IEnumerator InventoryMoveCoroutine()
    {
        CardUI.isCardMoving = true;

        yield return null;
        List<GameObject> _cards = CardInventory.Instance.CardsOnHand;
        yield return StartCoroutine(_Cardsmovemanager.CardsRotate(HandPosition, _cards, CardMoveDuration, MaxAngle, radius));
        foreach (GameObject card in _cards)
        {
            card.GetComponent<CardUI>().SetTransformofCard(card.transform.position, card.transform.rotation.eulerAngles);
        }

        CardUI.isCardMoving = false;
    }

    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void RPC_ChangeStatTurnButton()
    {
        if (TurnButton.GetComponentInChildren<Button>().isActiveAndEnabled)
        {
            TurnButton.GetComponentInChildren<Button>().enabled = false;
        }
        else
        {
            TurnButton.GetComponentInChildren<Button>().enabled = true;
        }
    }
    public void AddedCardMove()
    {
        StartCoroutine(AddedCardMoveCoroutine());
    }
    public IEnumerator AddedCardMoveCoroutine()
    {
        CardUI.isCardMoving = true;

        yield return null;
        List<GameObject> _cards = CardInventory.Instance.CardsOnHand;
        radius += 10f;
        radius = Mathf.Clamp(radius, 200, 300);
        yield return StartCoroutine(_Cardsmovemanager.CardsRotate(HandPosition, _cards, CardMoveDuration, MaxAngle, radius));

        foreach (GameObject card in _cards)
        {
            card.GetComponent<CardUI>().SetTransformofCard(card.transform.position, card.transform.rotation.eulerAngles);
        }
        CardUI.isCardMoving = false;
    }

    public void SetInfoPanProp(CardUI card)
    {
        CardData data = card.CardData;
        Debug.Log("Burdaaa");
        InfoPan.SetActive(true);
        InfoPan.transform.Find("InfoText").GetComponent<TextMeshProUGUI>().text = data?.CardDescription;

        InfoPan.transform.Find("Image").GetComponent<Image>().sprite = data?.sprite;

        string[] Values = data?.GetCardValuesString();

        TextMeshProUGUI targetText = InfoPan.transform.Find("CardText").GetComponent<TextMeshProUGUI>();

        string tmp_message = $"{data.CardName} \n";
        switch (data)
        {
            case AttackCard:
                tmp_message = tmp_message + $"Damage : {Values[0]} \n Health : {Values[1]} \n Armor Count : {Values[2]}";
                targetText.text = tmp_message;
                break;
            case DefenceCard when data as AttackPowerUpCard:

                tmp_message = tmp_message + $"Attack Power Count : {Values[1]}";
                targetText.text = tmp_message;

                break;
            case HealCard:
                tmp_message = tmp_message + $"Heal Count : {Values[0]}";
                targetText.text = tmp_message;
                break;
            case DefenceCard:
                tmp_message = tmp_message + $"Defence Count : {Values[1]}";
                targetText.text = tmp_message;
                break;
            default:
                tmp_message = "";
                targetText.text = tmp_message;
                break;


        }

    }
    public void SetInfoPanProp(Card card)
    {
        if (card == null)
        {
            Debug.LogError("Card is null!");
            return;
        }

        if (card.IsFrontly || Runner.LocalPlayer == card._CardData.Owner)
        {
            CardData data = card._CardData;

            InfoPan.SetActive(true);
            InfoPan.transform.Find("InfoText").GetComponent<TextMeshProUGUI>().text = data?.CardDescription;

            InfoPan.transform.Find("Image").GetComponent<Image>().sprite = data?.sprite;

            string[] Values = data?.GetCardValuesString();

            TextMeshProUGUI targetText = InfoPan.transform.Find("CardText").GetComponent<TextMeshProUGUI>();

            string tmp_message = $"{data.CardName} \n";
            switch (data)
            {
                case AttackCard:
                    tmp_message = tmp_message + $"Damage : {Values[0]} \n Health : {Values[1]} \n Armor Count : {Values[2]}";
                    targetText.text = tmp_message;
                    break;
                case DefenceCard when data as AttackPowerUpCard:

                    tmp_message = tmp_message + $"Attack Power Count : {Values[1]}";
                    targetText.text = tmp_message;

                    break;
                case HealCard:
                    tmp_message = tmp_message + $"Heal Count : {Values[0]}";
                    targetText.text = tmp_message;
                    break;
                case DefenceCard:
                    tmp_message = tmp_message + $"Defence Count : {Values[1]}";
                    targetText.text = tmp_message;
                    break;
                default:
                    tmp_message = "";
                    targetText.text = tmp_message;
                    break;


            }

    }

}

    public void ResetInfoPanProp()
    {
        InfoPan.SetActive(false);
    }
    #region Buttons
    public void IsFrontFace()
    {
        bool isFrontly = true;
        CardManager.Instance.CardPlayInitializer(isFrontly);
    }
    public void IsBackFace()
    {
        bool isFrontly = false;

        CardManager.Instance.CardPlayInitializer(isFrontly);
    }
    public void ReturnTheLobby()
    {
        SceneManager.LoadScene(sceneName: "Lobby");
        LobbyManager.Instance.ShutDown();
    }
    #endregion
}
