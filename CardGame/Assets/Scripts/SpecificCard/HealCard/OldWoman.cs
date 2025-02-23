using Fusion;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Heal Card", menuName = "Add Card/Heal Card/OldWoman")]
public class OldWoman : HealCard

{
    private void Awake()
    {
        CardType = CardType.HealingCard;
    }
    private void OnEnable()
    {
        TurnManager.OnRoundChanged += OnRoundChanged;

    }
    public override void CardEFfect(object AllyCard)
    {
    }
    public void OnRoundChanged(PlayerRef player)
    {
        if (GameManager.instance.GetACardofPlayer(Owner, this) == null)
        {
            TurnManager.OnRoundChanged -= OnRoundChanged;
            return;
        }
        if (player == Owner && GameManager.instance.GetACardofPlayer(Owner,this).IsFrontly)
        {
            List<Card> cards = GameManager.instance.GetCardsofPlayer(player);
            Debug.Log(cards.Count);
            if (cards.Count <= 0) return;
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i]._CardData.CardType != CardType.AttackCard)
                {
                    cards.Remove(cards[i]);
                }
            }

            foreach (var item in cards)
            {
               
                    NetworkObject targetAllie = item.GetComponent<NetworkObject>();
                    Card targetcard = targetAllie.GetComponent<Card>();
                    if (targetcard != null)
                    {
                        if (targetcard._CardData is AttackCard attackCard)
                        {
                            attackCard.Health += HealingAmount;
                            Debug.Log($"{targetcard.name} hedef ");
                            targetcard.UpgradeCardUnits();
                        }
                    }
                
            }

            

        }
    }
    public override CardData Clone()
    {
        OldWoman clone = CreateInstance<OldWoman>();

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
