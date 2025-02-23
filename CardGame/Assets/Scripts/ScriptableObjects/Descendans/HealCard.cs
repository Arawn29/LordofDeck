using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Heal Card", menuName = "Add Card/Heal Card/Normal Heal Card")]
public class HealCard : CardData
{


    [SerializeField] public int HealingAmount;

    public override string[] GetCardUIValues()
    {
        return new string[] { ManaCost.ToString(), HealingAmount.ToString(), "" };
    }
    public override string[] GetCardValuesString()
    {
        return new string[] { HealingAmount.ToString(), "" };
    }
    private void Awake()
    {
        CardTarget = EffectRange.Single;
        CardType = CardType.HealingCard;
    }

    public override void CardEFfect(object AllyCard)
    {
        if (AllyCard is Card allyCard)
        {
            if (allyCard != null && allyCard._CardData is AttackCard attack)
            {
                attack.Health = attack.Health + HealingAmount;
                allyCard.UpgradeCardUnits();
                List<Card> ourCards = GameManager.instance.GetCardsofPlayer(Owner);
                foreach (Card item in ourCards)
                {
                    Debug.Log($"Burada ve etki ettiði kart ise {allyCard._CardData.CardName}");
                    if (item._CardData == this && GameManager.instance.IsItServer())
                    {
                        item.DestroyCard();
                    }
                }
            }

        }

    }

    public override CardData Clone()
    {

        HealCard clone = CreateInstance<HealCard>();

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
