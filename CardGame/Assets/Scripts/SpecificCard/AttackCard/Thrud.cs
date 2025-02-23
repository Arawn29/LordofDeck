
using Fusion;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Card", menuName = "Add Card/Attack Card/Thrud")]

public class Thrud : AttackCard
{
    [SerializeField] public List<Card> TargetCards;

    // Saldýrdýðý düþmanýn bir tur boyunca saldýrmasýný engeller;
    private void Awake()
    {
        CardTarget = EffectRange.Single;
        TargetCards = new List<Card>();

    }
    public override void CardEFfect(object Enemycard)
    {
        if (Enemycard is Card enemyCard)
        {
            if (enemyCard._CardData is AttackCard attackCard)
            {
                #region ThisCardProcedure
                Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);
               
                    if (thisCard.IsFrontly == false)
                    {
                        thisCard.IsFrontly = true;
                    }
                    thisCard.isCardHasPlayed = true;

                
                thisCard.UpgradeCardUnits();
                #endregion
                #region EnemyCardProcedure
                if (Damage > attackCard.ArmorCount + attackCard.Health)
                {
                    int lifeDamage = Damage - attackCard.ArmorCount - attackCard.Health;
                    HealthManager.instance.RPC_HealthBarChange(enemyCard.CardPlayer, lifeDamage);
                    if (GameManager.instance.IsItServer())
                    {
                        enemyCard.DestroyCard();
                    }
                    return;
                }

                int remainingDamage = Mathf.Max(0, Damage - attackCard.ArmorCount); // 0 dönerse armor damagedan daha fazla demek;
                attackCard.Health = Mathf.Max(attackCard.Health - remainingDamage, 0);// 0 dan büyük bir þey dönerse Damage armordan yüksek demektir.
                attackCard.ArmorCount = Mathf.Max(attackCard.ArmorCount - Damage, 0);

                enemyCard.IsFrontly = enemyCard.IsFrontly || true;   // card.isFrontly = 0 || 1 => 1, 1||1 => 1 

                enemyCard.UpgradeCardUnits();

                if (attackCard.Health <= 0)
                {
                    if (GameManager.instance.IsItServer())
                    {
                        enemyCard.DestroyCard();
                    }
                }

                #endregion

                // Card Effect
                TargetCards.Add(enemyCard);
                TurnManager.OnRoundChanged += OnRoundChanged;

            }
        }
    }

    public void OnRoundChanged(PlayerRef player)
    {
        if (player != Owner)
        {

            foreach (var item in TargetCards)
            {
                Debug.Log($"Setting isCardHasPlayed for card: {item.name}");
                Debug.Log($"Before: {item.name} isCardHasPlayed = {item.isCardHasPlayed}");
                item.isCardHasPlayed = true;
                Debug.Log($"After: {item.name} isCardHasPlayed = {item.isCardHasPlayed}");

            }
        }
        else if (player == Owner)
        {
            TurnManager.OnRoundChanged -= OnRoundChanged;
            TargetCards.Clear();
        }


    }

    public override CardData Clone()
    {
        Thrud clone = CreateInstance<Thrud>();

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
