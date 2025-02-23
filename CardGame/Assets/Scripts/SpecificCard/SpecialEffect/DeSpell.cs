using UnityEngine;
[CreateAssetMenu(fileName = "New SpecialEffect Card", menuName = "Add Card/SpecialEffect Card/DeSpell")]
public class DeSpell : SpecialEffectCard
{
    public override void CardEFfect(object card)
    {
        if (GameManager.instance.IsItServer())
        {

            if (card is Card enemyCard)
            {
                if (enemyCard._CardData is not AttackCard)
                {
                    enemyCard.DestroyCard();
                    Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);
                    if (thisCard != null)
                    {
                        thisCard.DestroyCard();
                    }
                }

            }
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
        DeSpell clone = CreateInstance<DeSpell>();

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
