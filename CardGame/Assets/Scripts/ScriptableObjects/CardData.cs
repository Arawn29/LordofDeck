using Fusion;
using System;
using UnityEngine;

interface IClonable<T>
{
    public T Clone();
}
public enum EffectRange
{
    Single,
    Multiple,
    None
}
public enum CardType
{
    AttackCard,
    DefenceCard,
    HealingCard,
    SpecialEfectCard
}
[System.Serializable]
public class CardData : ScriptableObject, IClonable<CardData>
{

    public string CardName;
    public int ManaCost;
    [TextArea(15, 20)] public string CardDescription;
    public CardType CardType;
    public EffectRange CardTarget;
    public Sprite sprite;
    public int CardID;
    public NetworkObject ParentNetworkObject;
    //public bool isFrontly { get; set; }
    public PlayerRef Owner;
    public SoundData[] SoundData;

    public virtual void CardEFfect(object card) { }

    public virtual string[] GetCardValuesString() { return new string[] { }; }

    public virtual string[] GetCardUIValues() { return new string[] { }; }

    public virtual CardData Clone()
    {
        throw new NotImplementedException();
    }
    
    public virtual void  PlayEffectSound()
    {

    }
    public virtual void PlayHitttedSound()
    {

    }
    public virtual void PlayMoveSoundLoop()
    {

    }
    public virtual void StopSound()
    {

    }

}
