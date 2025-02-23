using UnityEngine;


[CreateAssetMenu(fileName = "New Attack Card", menuName = "Add Card/Attack Card/Drizzt NoUrden")]


public class DrizztNoUrden : AttackCard
{
    private void Awake()
    {
        CardTarget = EffectRange.Single;
    }

    public override CardData Clone()
    {
        DrizztNoUrden clone = CreateInstance<DrizztNoUrden>();

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
