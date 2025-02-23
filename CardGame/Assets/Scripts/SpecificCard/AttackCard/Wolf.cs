using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wolf Card", menuName = "Add Card/Attack Card/Wolf")]
public class Wolf : AttackCard
{
    public int lifeStealAmount;
    private void Awake()
    {
        CardTarget = EffectRange.Single;

    }
    public override void CardEFfect(object Enemycard)
    {
        if (Enemycard is Card card)
        {
            AttackCard enemyCard = card._CardData as AttackCard;
            if (enemyCard != null)
            {
                Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);
                #region ThisCardProcedure
                
                        if (thisCard.IsFrontly == false)
                        {
                            thisCard.IsFrontly = true;
                        }
                        Health += lifeStealAmount;
                        thisCard.isCardHasPlayed = true;
                        thisCard.UpgradeCardUnits();
             
                #endregion
                if (Damage > enemyCard.ArmorCount + enemyCard.Health)
                {
                    int lifeDamage = Damage - enemyCard.ArmorCount - enemyCard.Health;
                    HealthManager.instance.RPC_HealthBarChange(card.CardPlayer, lifeDamage);


                    if (GameManager.instance.IsItServer())
                    {
                        card.DestroyCard();
                    }

                    return;
                }

                int remainingDamage = Mathf.Max(0, Damage - enemyCard.ArmorCount); // 0 dönerse armor damagedan daha fazla demek;
                enemyCard.Health = Mathf.Max(enemyCard.Health - remainingDamage, 0);// 0 dan büyük bir þey dönerse Damage armordan yüksek demektir.
                enemyCard.ArmorCount = Mathf.Max(enemyCard.ArmorCount - Damage, 0);
              

                card.IsFrontly = card.IsFrontly || true;   // card.isFrontly = 0 || 1 => 1, 1||1 => 1 

                card.UpgradeCardUnits();

                if (enemyCard.Health <= 0)
                {
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
        Wolf clone = CreateInstance<Wolf>();
        clone.lifeStealAmount = this.lifeStealAmount;

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
