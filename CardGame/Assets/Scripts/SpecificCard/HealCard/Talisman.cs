using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Heal Card", menuName = "Add Card/Heal Card/Talisman")]
public class Talisman : HealCard
{
    private void Awake()
    {
        CardType = CardType.HealingCard;
        CardTarget = EffectRange.Multiple;
    }

    public override void CardEFfect(object AllyCard)
    {
        
            List<Card> attackCards = GameManager.instance.GetSpecificCardsofPlayer(CardType.AttackCard, Owner);
            foreach (Card card in attackCards)
            {

                if (card._CardData is AttackCard targetCard)
                {
                    targetCard.Health += HealingAmount;
                    card.UpgradeCardUnits();
                }

            }
            if (GameManager.instance.IsItServer())
            {
              Card ThisCard = GameManager.instance.GetACardofPlayer(Owner, this);
                if (ThisCard != null)
                {
                    ThisCard.DestroyCard();
                }
            }

        
    }


    public override CardData Clone()
    {

        Talisman clone = CreateInstance<Talisman>();

        clone.HealingAmount = this.HealingAmount;
        clone.CardDescription = this.CardDescription;
        clone.CardID = this.CardID;
        clone.CardName = this.CardName;
        clone.CardType = this.CardType;
        clone.sprite = this.sprite;
        clone.ManaCost = this.ManaCost;
        return clone;

    }
}
