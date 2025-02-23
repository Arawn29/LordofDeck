using Fusion;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpecialEffect Card", menuName = "Add Card/SpecialEffect Card/DrawCard")]
public class DrawCard : SpecialEffectCard
{
    public int drawCount;

    public override void CardEFfect(object card)
    {
        if (card is PlayerRef player)
        {
            if (player == Owner)
            {

                for (int i = 0; i < drawCount; i++)
                {
                    if (Deck.Instance.CardCountinDeck > 0)
                    {
                        CardInventory.Instance.AddCardForInventory();
                    }
                }

                CanvaSys.Instance.InventoryMove();
            }
            if (GameManager.instance.IsItServer())
            {
                Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);
                if (thisCard != null) { thisCard.DestroyCard(); }
            }
        }
    }
    public override CardData Clone()
    {
        DrawCard clone = CreateInstance<DrawCard>();
        clone.drawCount = drawCount;
        clone.CardDescription = this.CardDescription;
        clone.SoundData = this.SoundData;
        clone.CardID = this.CardID;
        clone.CardName = this.CardName;
        clone.CardType = this.CardType;
        clone.sprite = this.sprite;
        clone.ManaCost = this.ManaCost;
        return clone;
    }
}
