using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New SpecialEffect Card", menuName = "Add Card/SpecialEffect Card/BlackHole")]
public class BlackHole : SpecialEffectCard
{
    public override void CardEFfect(object card)
    {
        if (GameManager.instance.IsItServer())
        {

            if (card is List<Card> allCards)
            {
                foreach (var item in allCards)
                {
                    if (item._CardData != this)
                    {
                        item.DestroyCard();
                    }
                }
                Card thisCard = GameManager.instance.GetACardofPlayer(Owner,this);
                thisCard.DestroyCard();
            }
        }
    }
    public override CardData Clone()
    {
        BlackHole clone = CreateInstance<BlackHole>();

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