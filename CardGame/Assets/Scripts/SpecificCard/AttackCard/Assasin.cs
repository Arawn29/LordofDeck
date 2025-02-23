using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Attack Card", menuName = "Add Card/Attack Card/Assasssin")]
public class Assasin : AttackCard

{
    public override void CardEFfect(object Enemycard)
    {
        if (Enemycard is Card card)
        {
            AttackCard enemyCard = card._CardData as AttackCard;
            if (enemyCard != null)
            {
                #region ThisCardProcedure
                Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);

                if (thisCard.IsFrontly == false) { thisCard.IsFrontly = true; }
                thisCard.isCardHasPlayed = true;
                thisCard.UpgradeCardUnits();
                #endregion
                if (Damage > enemyCard.ArmorCount + enemyCard.Health)
                {
                    int lifeDamage = Damage - enemyCard.ArmorCount - enemyCard.Health;
                    HealthManager.instance.RPC_HealthBarChange(card.CardPlayer, lifeDamage);
                    if (GameManager.instance.IsItServer())
                    {
                        thisCard.isCardHasPlayed = false;   
                        
                        card.DestroyCard();
                    }
                    return;
                }

                int remainingDamage = Mathf.Max(0, Damage - enemyCard.ArmorCount); // 0 dönerse armor damagedan daha fazla demek;
                enemyCard.Health = Mathf.Max(enemyCard.Health - remainingDamage, 0);// 0 dan büyük bir þey dönerse Damage armordan yüksek demektir.
                enemyCard.ArmorCount = Mathf.Max(enemyCard.ArmorCount - Damage, 0);

                card.IsFrontly = card.IsFrontly || true;   // card.isFrontly = 0 || 1 => 1, 1||1 => 1 

                card.UpgradeCardUnits();

                if (enemyCard.Health <= 0 && GameManager.instance.IsItServer())
                {
                    thisCard.isCardHasPlayed = false;
                    card.DestroyCard();
                }


            }

        }
    }



    public override CardData Clone()
    {
        Assasin clone = CreateInstance<Assasin>();

        clone.Health = this.Health;
        clone.Damage = this.Damage;
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
