using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Defence Card", menuName = "Add Card/Defence Card/ Iron Tree Extract")]
public class IronTreeExtract : DefenceCard
{
    private void Awake()
    {
        CardTarget = EffectRange.Multiple;

    }

    public override void CardEFfect(object AllyCard)
    {

        List<Card> attackCards = GameManager.instance.GetSpecificCardsofPlayer(CardType.AttackCard, Owner);

        foreach (Card card in attackCards)
        {

            if (card._CardData is AttackCard targetCard)
            {
                targetCard.ArmorCount += ArmorCount;
                card.UpgradeCardUnits();
            }

        }
        if (GameManager.instance.IsItServer())
        {
            Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);
            thisCard.DestroyCard();
        }


    }
    public override CardData Clone()
    {
        IronTreeExtract clone = CreateInstance<IronTreeExtract>();
        clone.SoundData = this.SoundData;
        clone.ArmorCount = this.ArmorCount;
        clone.CardDescription = this.CardDescription;
        clone.CardID = this.CardID;
        clone.CardName = this.CardName;
        clone.CardType = this.CardType;
        clone.sprite = this.sprite;
        clone.ManaCost = this.ManaCost;
        return clone;
    }
}
