using UnityEngine;


[CreateAssetMenu(fileName = "New SpecialEffect Card", menuName = "Add Card/SpecialEffect Card/Courage")]
public class Courage : SpecialEffectCard
{
    public override void CardEFfect(object card)
    {
        if (GameManager.instance.IsItServer())
        {

            if (card is Card allyCard)
            {
                allyCard.isCardHasPlayed = false;
                Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);
                if (thisCard != null)
                {
                    thisCard.DestroyCard();
                }

            }
        }
    }

    public override CardData Clone()
    {
        Courage clone = CreateInstance<Courage>();

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
