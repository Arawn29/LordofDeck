using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Card", menuName = "Add Card/Attack Card/Thalion")]

public class Thalion : AttackCard
{
    public int earnedHealth;
    public override void CardEFfect(object Enemycard)
    {
        // Her �ld�rd���nde +3 can kazan�r.

        if (Enemycard is Card card)
        {
            Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);
            #region ThisCardProcedure
           
            if(thisCard.IsFrontly == false) { thisCard.IsFrontly = true; }
            thisCard.isCardHasPlayed = true;
            thisCard.UpgradeCardUnits();
            #endregion

            AttackCard enemyCard = card._CardData as AttackCard;
            if (enemyCard != null)
            {
                if (Damage > enemyCard.ArmorCount + enemyCard.Health)
                {
                    int lifeDamage = Damage - enemyCard.ArmorCount - enemyCard.Health;
                    HealthManager.instance.RPC_HealthBarChange(card.CardPlayer, lifeDamage);
                    Health += earnedHealth;
                    thisCard.UpgradeCardUnits();
                    if (GameManager.instance.IsItServer())
                    {
                        card.DestroyCard();
                    }
                    return;
                }

                int remainingDamage = Mathf.Max(0, Damage - enemyCard.ArmorCount); // 0 d�nerse armor damagedan daha fazla demek;
                enemyCard.Health = Mathf.Max(enemyCard.Health - remainingDamage, 0);// 0 dan b�y�k bir �ey d�nerse Damage armordan y�ksek demektir.
                enemyCard.ArmorCount = Mathf.Max(enemyCard.ArmorCount - Damage, 0);

                card.IsFrontly = card.IsFrontly || true;   // card.isFrontly = 0 || 1 => 1, 1||1 => 1 

                card.UpgradeCardUnits();

               
                if (enemyCard.Health <= 0)
                {
                    Health += earnedHealth;
                   
                    thisCard.UpgradeCardUnits();
                    if (GameManager.instance.IsItServer())
                    {
                        card.DestroyCard();
                    }
                }


            }

        }
    }
    public override CardData Clone()
    {
        Thalion clone = CreateInstance<Thalion>();
        clone.earnedHealth = this.earnedHealth;

        clone.SoundData = this.SoundData;
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
