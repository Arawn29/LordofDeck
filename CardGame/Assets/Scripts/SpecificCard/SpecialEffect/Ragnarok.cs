using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "New SpecialEffect Card", menuName = "Add Card/SpecialEffect Card/Ragnarok")]
public class Ragnarok : SpecialEffectCard
{
    public override void CardEFfect(object card)
    {
        if (GameManager.instance.IsItServer())
        {

        }
        if (card is List<Card> attackCards)
        {
            foreach (var _card in attackCards)
            {
                _card.DestroyCard();
            }
            Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);
            if (thisCard != null)
            {
                thisCard.DestroyCard();
            }
            //DestroyThisCard();
        }
    }

    //public async void DestroyThisCard()
    //{
    //    await Task.Delay(1000);
    //    Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);
    //    thisCard.DestroyCard();
    //}
    public override CardData Clone()
    {
        Ragnarok clone = CreateInstance<Ragnarok>();

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
