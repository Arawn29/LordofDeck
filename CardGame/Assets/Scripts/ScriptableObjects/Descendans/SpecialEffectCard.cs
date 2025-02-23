using UnityEngine;

public enum SpecialCardTarget
{
    Enemy,
    Ally,
    None,
    All
}

[CreateAssetMenu(fileName = "New SpecialEffect Card", menuName = "Add Card/SpecialEffect Card")]
public class SpecialEffectCard : CardData
{
    private void Awake()
    {
        CardType = CardType.SpecialEfectCard;
    }

    public override string[] GetCardUIValues()
    {
        return new string[] { ManaCost.ToString(), "", "" };
    }
    public override string[] GetCardValuesString()
    {
        return new string[] { "", "" };
    }

    public override CardData Clone()
    {
        SpecialEffectCard clone = CreateInstance<SpecialEffectCard>();

        clone.CardDescription = this.CardDescription;
        clone.SoundData = this.SoundData;
        clone.CardID = this.CardID;
        clone.CardName = this.CardName;
        clone.CardType = this.CardType;
        clone.sprite = this.sprite;
        clone.ManaCost = this.ManaCost;
        return clone;
    }
}
