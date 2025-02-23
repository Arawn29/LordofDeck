using TMPro;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public static Deck Instance;
    public int CardCountinDeck;
    [SerializeField] private TextMeshProUGUI[] _deckValueTexts = new TextMeshProUGUI[2];
    public static bool isButtonPressed = true;

    private void Awake()
    {
        Instance = this;
    }

    public void DeckSetup()
    {
        {
            if (CardInventory.Instance.CardsOnDeck.Count >= 0)
            {

                CardCountinDeck = CardInventory.Instance.CardsOnDeck.Count;
                

                foreach (TextMeshProUGUI text in _deckValueTexts)
                {
                    text.text = CardCountinDeck.ToString();
                }
            }
        }
    }
    public void DeckButtonPressed(int drawnCardCount)
    {

        if (CardInventory.Instance.CardsOnDeck.Count > 0 && !isButtonPressed)
        {
            isButtonPressed = true;
            CardCountinDeck -= drawnCardCount;

            foreach (TextMeshProUGUI deck in _deckValueTexts) { deck.text = "" + CardCountinDeck; }

            CardInventory.Instance.AddCardForInventory();

          CanvaSys.Instance.AddedCardMove();

        }



    }


}
