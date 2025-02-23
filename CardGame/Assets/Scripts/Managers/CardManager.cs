using Fusion;
using System.Collections;
using UnityEngine;



public class CardManager : NetworkBehaviour
{
    [Header("localComponents")]
    public static CardManager Instance;
    private GameObject DecideCardFacePan;
    //[Networked, Capacity(3)]
    //NetworkArray<NetworkString<_4>> CardMessages { get; } = MakeInitializer(new NetworkString<_4>[] { "", "", "" }); // _4 = 9999 a kadar string
    string[] CardMessages = new string[3];
    [Header("-----")]
    private CardInformations pickedCard;
    [Networked] public bool isInProccces { get; set; }


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        DecideCardFacePan = CanvaSys.Instance.SetFaceOfCardPanel;
        DecideCardFacePan.SetActive(false);


    }


    //----------------------------------------------------------CARD CREATION-------------------------------------------------
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))    // Ýptal etme 
        {
            DecideCardFacePan.SetActive(false);
            CanvaSys.Instance.InventoryMove();
        }
    }
    public NetworkObject CreateCard(NetworkObject CardArea, PlayerRef player)
    {

        NetworkObject CreatedCard;
        CreatedCard = Runner.Spawn(PrefabManager.instance.CardPrefab, CardArea.transform.position, Quaternion.identity, player);
        return CreatedCard;
    }
    public void SetTheCardInfos(CardInformations cardInfos)
    {
        pickedCard = cardInfos;
    }
    public void SetFaceDeciderPan()
    {
        DecideCardFacePan.SetActive(true);
    }

    // butona basýldýðýnda çalýþtýrýlan fonksiyon 
    public void CardPlayInitializer(bool isFrontly)
    {

        DecideCardFacePan.SetActive(false);

        GameManager.instance.RPC_CheckCardStateability(isFrontly, pickedCard.player, pickedCard.CardData.CardID, pickedCard.AreaNetworkObj, pickedCard.CardUniqeID);
    }


    //----------------------------------------------------------CARD SETUP-------------------------------------------------

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_CardInitialize(NetworkObject @object)
    {
        StartCoroutine(CardInitialize(@object));
    }

    public void CardUpdateSync(NetworkObject @object)
    {
        StartCoroutine(CardUpdate(@object));
    }
    #region MainCoroutines
    IEnumerator CardInitialize(NetworkObject @object)
    {
        yield return StartCoroutine(CardDataSync(@object));
        yield return StartCoroutine(SetFaceSprite(@object));
        yield return StartCoroutine(UpdateCardText(@object));
    }
    IEnumerator CardUpdate(NetworkObject @object)
    {
        yield return StartCoroutine(SetFaceSprite(@object));
        yield return StartCoroutine(UpdateCardText(@object));
    }
    #endregion

    #region SubCoroutines

    IEnumerator CardDataSync(NetworkObject targetCardObject)
    {
        while (true)
        {
            bool isInProccesDone = isCardSyncDone();

            bool isCardSyncDone()
            {
                Card card = targetCardObject.gameObject.GetComponent<Card>();
                if (card != null)
                {
                    CardData CardDatainMagazine = GameManager.instance.FindCardFromCardDataMagazine(card.CardID);
                    CardData CloneCardData = CardDatainMagazine.Clone();

                    card._CardData = CloneCardData;
                    card._CardData.Owner = targetCardObject.InputAuthority;
                    card._CardData.ParentNetworkObject = card._ParentNetworkObject;

                    if (card._CardData == CloneCardData && card._CardData.ParentNetworkObject == card._ParentNetworkObject && targetCardObject.InputAuthority == card._CardData.Owner)
                    {
                        return true;
                    }
                    else
                    {
                        bool isit = card._CardData == CloneCardData ? true : false;
                        bool isit2 = card._CardData.ParentNetworkObject == card._ParentNetworkObject ? true : false;
                        Debug.Log("Þartlar saðlanamdýdatasync");
                        Debug.Log($"{CloneCardData.Owner}");
                        Debug.Log($"{isit} card._carddata ile clone card data eþleþme durumu");
                        Debug.Log($"{isit2} card._carddata ile clone card data eþleþme durumu");
                        Debug.Log($"{card.CardID} kart id si ");
                        return false;
                    }
                }
                else
                {
                    Debug.Log("Þartlar saðlanamdýdatasync");
                    return false;
                }
            }
            if (isInProccesDone)
            {
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }

        }
    }
    IEnumerator SetFaceSprite(NetworkObject targetCardObject)
    {

        while (true)
        {
            bool isProccesDone = isFaceSpriteChanged();
            if (isProccesDone)
            {
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
            bool isFaceSpriteChanged()
            {
                Card card = targetCardObject.GetComponent<Card>();

                if (card.IsFrontly)
                {
                    targetCardObject.transform.Find("GFX").GetComponent<SpriteRenderer>().sprite = card._CardData.sprite;
                    if (targetCardObject.transform.Find("GFX").GetComponent<SpriteRenderer>().sprite == card._CardData.sprite)
                    {
                        return true;
                    }
                    else
                    {
                        Debug.Log("Þartlar saðlanamdýsprite");
                        return false;
                    }
                }
                if (!card.IsFrontly)
                {
                    targetCardObject.transform.Find("GFX").GetComponent<SpriteRenderer>().sprite = PrefabManager.instance.CardBackFace;
                    if (targetCardObject.transform.Find("GFX").GetComponent<SpriteRenderer>().sprite == PrefabManager.instance.CardBackFace)
                    {
                        return true;
                    }
                    else
                    {
                        Debug.Log("Þartlar saðlanamdýsprite");
                        return false;
                    }
                }
                else
                {
                    Debug.Log("Þartlar saðlanamdý");
                    return false;
                }
            }

        }

    }
    IEnumerator UpdateCardText(NetworkObject targetCardObject)
    {
        while (true)
        {
            bool isProccesCompleteed = isCardTextUpdate();


            if (isProccesCompleteed)
            {
                break;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
            bool isCardTextUpdate()
            {
                Card card = targetCardObject?.GetComponent<Card>();
                if (card == null) { return false; }
                if (card != null)
                {
                    string[] messages = card._CardData.GetCardValuesString();
                    for (int i = 0; i < CardMessages.Length; i++)
                    {
                        if (i < messages.Length)
                        {
                            CardMessages.SetValue(messages[i], i);
                            //CardMessages.set(i, messages[i]);
                        }

                    }

                    if (card.IsFrontly)
                    {

                        //TextSettings
                        for (int i = 0; i < CardMessages.Length; i++)
                        {
                            if (card.cardTexts[i] != null && CardMessages[i] != null)
                            {
                                card.cardTexts[i].text = CardMessages[i].ToString();
                            }
                        }
                        return true;
                    }
                    if (!card.IsFrontly && targetCardObject.InputAuthority == Runner.LocalPlayer)
                    {
                        //TextSettings
                        for (int i = 0; i < CardMessages.Length; i++)
                        {
                            if (card.cardTexts[i] != null)
                            {
                                card.cardTexts[i].text = CardMessages[i].ToString();
                            }
                        }
                        return true;
                    }
                    else return false;
                }
                else return false;
            }

        }
    }

    #endregion

    //----------------------------------------//----------------------------------------//-------------------------------//

    //------------------------------------------------Sound Management------------------------------------------------------------
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_PlaySound(int enumIndex, NetworkObject targetCardObj)
    {
        if (targetCardObj != null)
        {
            Card card = targetCardObj.GetComponent<Card>();

            if (card != null)
            {

                CardData carddata = GameManager.instance.FindCardFromCardDataMagazine(card.CardID);
                switch (enumIndex)
                {
                    case (int)ClipType.GiveEffect:
                        carddata.PlayEffectSound();
                        break;
                    case (int)ClipType.Move:
                        carddata.PlayMoveSoundLoop();
                        break;
                    case (int)ClipType.TakeEffect:
                        carddata.PlayHitttedSound();
                        break;
                }
            }
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_StopSound()
    {
        SoundManager.Instance.StopSound();
    }
}

