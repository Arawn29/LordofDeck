using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Attack Card", menuName = "Add Card/Attack Card/Blind Eve")]
public class Lilith : AttackCard
{
    private void Awake()
    {
        CardTarget = EffectRange.Single;
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
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
                        card.DestroyCard();
                    }
                    return;
                }
                int remainingDamage = Mathf.Max(0, Damage - enemyCard.ArmorCount); // 0 dönerse armor damagedan daha fazla demek;
                enemyCard.Health = Mathf.Max(enemyCard.Health - remainingDamage, 0);// 0 dan büyük bir þey dönerse Damage armordan yüksek demektir.
                enemyCard.ArmorCount = Mathf.Max(enemyCard.ArmorCount - Damage, 0);

                card.IsFrontly = card.IsFrontly || true;   // card.isFrontly = 0 || 1 => 1, 1||1 => 1 

                card.UpgradeCardUnits();


                AttackAlly();


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

    private void AttackAlly()
    {
        List<Card> attackCards = GameManager.instance.GetSpecificCardsofPlayer(CardType.AttackCard, Owner);

        for (int i = 0; i < attackCards.Count; i++)
        {

            if (attackCards[i]._CardData is Lilith)
            {
                attackCards.RemoveAt(i);
            }

        }

        if (attackCards.Count > 0)
        {
            int randomAlly = Random.Range(0, attackCards.Count);

            Card AllyCard = attackCards[randomAlly];
            AllyCard.IsFrontly = AllyCard.IsFrontly || true;   // card.isFrontly = 0 || 1 => 1, 1||1 =>
            AttackCard attackCard = AllyCard._CardData as AttackCard;

            attackCard.Health -= (int)(Damage / 2);
            AllyCard.UpgradeCardUnits();
            if (attackCard.Health <= 0)
            {
                if (GameManager.instance.IsItServer())
                {
                    AllyCard.DestroyCard();
                }
            }
        }


    }
    public override CardData Clone()
    {
        Lilith clone = CreateInstance<Lilith>();

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
        clone.Owner = this.Owner;
        return clone;
    }
}

