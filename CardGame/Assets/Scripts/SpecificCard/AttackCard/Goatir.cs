using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


[CreateAssetMenu(fileName = "New Attack Card", menuName = "Add Card/Attack Card/Goatir")]

public class Goatir : AttackCard
{

    int firstAttackAmount;
    /// <summary>
    /// Sahada ki tüm düþmanlara saldýrýr.
    /// </summary>
    private void Awake()
    {
        CardTarget = EffectRange.Single;
    }

    public override void CardEFfect(object Enemycard)
    {
        if (Enemycard is Card _enemyCard)
        {
            #region ThisCardProcedure
            Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);
            if (thisCard.IsFrontly == false) { thisCard.IsFrontly = true; }
            thisCard.isCardHasPlayed = true;
            thisCard.UpgradeCardUnits();
            #endregion
            List<Card> enemyCards = GameManager.instance.GetSpecificCardsofPlayer(CardType.AttackCard,_enemyCard.CardPlayer);
            foreach (Card item in enemyCards)
            {
                if (item._CardData == _enemyCard._CardData)
                {
                    enemyCards.Remove(item);
                    break;
                }
            }

            if (enemyCards.Count > 0) { AttackMultipleTargets(enemyCards); }

            AttackCard enemyCard = _enemyCard._CardData as AttackCard;
            if (enemyCard != null)
            { 
                if (Damage > enemyCard.ArmorCount + enemyCard.Health)
                {
                    int lifeDamage = Damage - enemyCard.ArmorCount - enemyCard.Health;
                    HealthManager.instance.RPC_HealthBarChange(_enemyCard.CardPlayer, lifeDamage);
                    if (GameManager.instance.IsItServer())
                    {
                        _enemyCard.DestroyCard();
                    }
                    return;
                }
                int remainingDamage = Mathf.Max(0, Damage - enemyCard.ArmorCount); // 0 dönerse armor damagedan daha fazla demek;
                enemyCard.Health = Mathf.Max(enemyCard.Health - remainingDamage, 0);// 0 dan büyük bir þey dönerse Damage armordan yüksek demektir.
                enemyCard.ArmorCount = Mathf.Max(enemyCard.ArmorCount - Damage, 0);

                _enemyCard.IsFrontly = _enemyCard.IsFrontly || true;   // card.isFrontly = 0 || 1 => 1, 1||1 => 1 


                _enemyCard.UpgradeCardUnits();

                if (enemyCard.Health <= 0 && GameManager.instance.IsItServer())
                {
                    _enemyCard.DestroyCard();
                }


            }


        }
    }
    public void AttackMultipleTargets(List<Card> enemyCards)
    {
        for (int i = 0; i < enemyCards.Count; i++)
        {

            AttackCard attackCard = enemyCards[i]._CardData as AttackCard;

            if (firstAttackAmount > attackCard.Health + attackCard.ArmorCount)
            {
                int lifeDamage = firstAttackAmount - attackCard.ArmorCount - attackCard.Health;
                HealthManager.instance.RPC_HealthBarChange(enemyCards[i].CardPlayer, lifeDamage);
                if (GameManager.instance.IsItServer())
                {
                    enemyCards[i].DestroyCard();
                }
                return;
            }
            int remainingDamage = Mathf.Max(0, firstAttackAmount - attackCard.ArmorCount); // 0 dönerse armor damagedan daha fazla demek;
            attackCard.Health = Mathf.Max(attackCard.Health - remainingDamage, 0);// 0 dan büyük bir þey dönerse Damage armordan yüksek demektir.
            attackCard.ArmorCount = Mathf.Max(attackCard.ArmorCount - firstAttackAmount, 0);
            Debug.Log($"Hasar yiyen kartýn mevcut armoru{attackCard.ArmorCount}, caný {attackCard.Health} ");

            if (enemyCards[i].IsFrontly == false) enemyCards[i].IsFrontly = true;

            enemyCards[i].UpgradeCardUnits();
            if (attackCard.Health <= 0)
            {
                if (GameManager.instance.IsItServer())
                {
                    enemyCards[i].DestroyCard();
                }
            }
        }
    }
    public override CardData Clone()
    {
        Goatir clone = CreateInstance<Goatir>();
        clone.firstAttackAmount = this.Damage;
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
