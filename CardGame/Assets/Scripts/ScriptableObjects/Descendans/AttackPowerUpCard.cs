using UnityEngine;



[CreateAssetMenu(fileName = "New AttackPowerUp Card", menuName = "Add Card/AttackPowerUp Card / Normal AttackPowerUp")]
public class AttackPowerUpCard : DefenceCard
{
    public int attackPowerUpAmount;

    public override string[] GetCardUIValues()
    {
        return new string[] { ManaCost.ToString(),"", "" };
    }
    public override string[] GetCardValuesString()
    {
        return new string[] { "", attackPowerUpAmount.ToString(), "" };
    }
    public override void CardEFfect(object AllyCard)
    {
        if (AllyCard is Card allyCard)
        {
            if (allyCard._CardData is AttackCard attack)
            {
                attack.Damage += attackPowerUpAmount;
                allyCard.UpgradeCardUnits();
                if (GameManager.instance.IsItServer())
                {
                    Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);
                    thisCard.DestroyCard();
                }
            }

        }
    }

    public override CardData Clone()
    {
        AttackPowerUpCard clone = CreateInstance<AttackPowerUpCard>();

        clone.attackPowerUpAmount = this.attackPowerUpAmount;
        clone.CardDescription = this.CardDescription;
        clone.CardID = this.CardID;
        clone.CardName = this.CardName;
        clone.CardType = this.CardType;
        clone.sprite = this.sprite;
        clone.ManaCost = this.ManaCost;
        return clone;
    }

}
