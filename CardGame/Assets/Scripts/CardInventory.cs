using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CardInventory : MonoBehaviour
{
    public static CardInventory Instance;

    [Header("InventorySysnc")]

    public static int firstRoundCardStack = 6;    // �lk round olu�acak kart say�s� 

    public List<GameObject> CardsOnHand = new List<GameObject>();  // Envanterdeki kartlar� i�erisinde tutar. 

    public List<CardData> CardsOnDeck = new List<CardData>();    // T�m kartlar - Envanter Kartlar� 

    [SerializeField] private Transform CardsSpriteParent;   // Sahnede olu�turulan kartlar�n parent � 

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
       
        foreach (CardData data in CardMagazine.instance.AllCards)  // IMPORTANTE :!! Deep clone yapman laz�m �b�r t�rl� bir listi bir liste e�leyince sadece referans
        {                                                         // oluyor yeni bir list olu�muyor.
            CardsOnDeck.Add(data);
            Debug.Log("Eldeki kart say�s�" + CardsOnDeck.Count);
        }

    }

    private void Start()
    {
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
    }

    public void CreatingInventory()
    {
        CardsOnHand.Clear();

        for (int i = 0; i < firstRoundCardStack; i++)
        {
            if (CardsOnDeck.Count > 0)
            {
                int random = UnityEngine.Random.Range(0, CardsOnDeck.Count);

                CardData card = CardsOnDeck[random];


                // Kart�n sahneye eklenmesi
                CardSpriteGeneration cardGeneration = new CardSpriteGeneration(card, CardsSpriteParent);
                GameObject newCardObj = cardGeneration.ReturnCratedCard();



                CardsOnDeck.RemoveAt(random);
                Debug.Log("Eldeki kart say�s�" + CardsOnDeck.Count);
                CardsOnHand.Add(newCardObj);
                Deck.Instance.DeckSetup();
                CanvaSys.Instance.InventoryMove();
            }
        }

    }

    public void AddCardForInventory()
    {
        if (CardsOnDeck.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, CardsOnDeck.Count);
            CardData card = CardsOnDeck[random];

            CardSpriteGeneration cardGeneration = new CardSpriteGeneration(card, CardsSpriteParent);
            GameObject newCardObj = cardGeneration.ReturnCratedCard();

            CardsOnDeck.RemoveAt(random);
            CardsOnHand.Add(newCardObj);
        }


    }


    public class CardSpriteGeneration
    {
        CardData card;
        Transform ParentObject;
        GameObject CardVisual;

        public CardSpriteGeneration(CardData card, Transform ParentObject)
        {
            this.card = card;
            this.ParentObject = ParentObject;
            CardGenerating();

        }

        public void CardGenerating()
        {
            GameObject CardFrame = CardMagazine.instance.CardFrame.Dictionary[card.CardType];
            CardVisual = Instantiate(CardFrame, ParentObject.transform.position, Quaternion.identity, ParentObject);

            CardVisual.transform.Find("DescriptionArea").GetChild(0).GetComponent<TextMeshProUGUI>().text = card.CardDescription;
          
            CardVisual.transform.Find("NameArea").GetChild(0).GetComponent<TextMeshProUGUI>().text = card.CardName;
            CardVisual.transform.Find("Picture").GetComponent<Image>().sprite = card.sprite;

            #region cardTextSet
            string[] cardtext = card.GetCardUIValues();
          
            var textManaArea = CardVisual.transform.Find("ManaArea")?.GetChild(0)?.GetComponent<TextMeshProUGUI>();
            if (textManaArea != null) textManaArea.text = cardtext[0];

            var textEffectArea = CardVisual.transform.Find("EffectArea")?.GetChild(0)?.GetComponent<TextMeshProUGUI>();
            if (textEffectArea != null) textEffectArea.text = cardtext[1];

            var textHealthArea = CardVisual.transform.Find("HealthArea")?.GetChild(0)?.GetComponent<TextMeshProUGUI>();
            if(textHealthArea != null) textHealthArea.text = cardtext[2];
            #endregion

            if (CardVisual.TryGetComponent(out CardUI component))
            {
                component.CardData = card;
            }


        }

        public GameObject ReturnCratedCard()
        {
            return CardVisual;
        }

    }
}

