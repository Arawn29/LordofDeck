
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Card", menuName = "Add Card/Attack Card/Death")]
public class Death : AttackCard

{
    public override string[] GetCardValuesString()
    {
        return new string[] { "∞", Health.ToString(), ArmorCount.ToString() };
    }
    public override string[] GetCardUIValues()
    {
        return new string[] { ManaCost.ToString(), "∞", Health.ToString() };
    }
    private void Awake()
    {
        CardTarget = EffectRange.Single;
    }
    public override void CardEFfect(object Enemycard)
    {
        if (Enemycard is Card enemycard)
        {
            #region ThisCardProcedure
            Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);

            if (thisCard.IsFrontly == false) { thisCard.IsFrontly = true; }
            thisCard.isCardHasPlayed = true;
            #endregion
            if (GameManager.instance.IsItServer())
            {
                enemycard.DestroyCard();
            }

        }
    }

    public override CardData Clone()
    {
        Death clone = CreateInstance<Death>();

        clone.SoundData = this.SoundData;
        clone.Health = this.Health;
        clone.CardDescription = this.CardDescription;
        clone.CardID = this.CardID;
        clone.ArmorCount = this.ArmorCount;
        clone.CardName = this.CardName;
        clone.CardType = this.CardType;
        clone.sprite = this.sprite;
        clone.ManaCost = this.ManaCost;
        return clone;
    }
}
