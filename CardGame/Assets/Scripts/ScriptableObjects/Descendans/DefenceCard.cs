using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Defence Card", menuName = "Add Card/Defence Card / Normal Defence Card")]
public class DefenceCard : CardData
{

    [SerializeField] public int ArmorCount;

    public override string[] GetCardUIValues()
    {
        return new string[] { ManaCost.ToString(), ArmorCount.ToString(), "" };
    }
    public override string[] GetCardValuesString()
    {
        return new string[] {"",ArmorCount.ToString(), "" };
    }
    private void Awake()
    {
        CardType = CardType.DefenceCard;
        CardTarget = EffectRange.Single;
    }
    public override void CardEFfect(object AllyCard)
    {
        if (AllyCard is Card allyCard)
        {
            if (allyCard != null && allyCard._CardData is AttackCard attack)
            {
                attack.ArmorCount = attack.ArmorCount + ArmorCount;
                allyCard.UpgradeCardUnits();
                List<Card> ourCards = GameManager.instance.GetCardsofPlayer(Owner);
                foreach (Card item in ourCards)
                {
                    Debug.Log($"Burada ve etki ettiði kart ise {allyCard._CardData.CardName}");
                    if (item._CardData == this && GameManager.instance.IsItServer())
                    {
                        item.DestroyCard();
                    }
                }
            }
           
        }

    }
    #region SoundEffects
    public override void PlayEffectSound()      // PLay in order 
    {
        List<AudioClip> attackClips = new List<AudioClip>();
        foreach (var item in SoundData)
        {
            if (item.ClipType == ClipType.GiveEffect)
            {
                attackClips.Add(item.AudioClip);
            }
        }
        SoundManager.Instance.PlaySoundsTogether(attackClips);
    }
    public override void PlayHitttedSound()     // Vurulma efekti AudioClip = 1 
    {
        //List<AudioClip> hitClips = new List<AudioClip>();
        AudioClip clip = null;
        foreach (var item in SoundData)
        {
            if (item.ClipType == ClipType.TakeEffect)
            {
                clip = item.AudioClip;
            }
        }
        SoundManager.Instance.PlaySound(clip);
    }
    public override void PlayMoveSoundLoop()        // Move sound loop 
    {
        AudioClip clip = null;
        foreach (var item in SoundData)
        {
            if (item.ClipType == ClipType.Move)
            {
                clip = item.AudioClip;
            }
        }
        SoundManager.Instance.PlaySoundLoop(clip);
    }

    public override void StopSound()
    {
        SoundManager.Instance.StopSound();
    }
    #endregion

    public override CardData Clone()
    {
        DefenceCard clone = CreateInstance<DefenceCard>();
        clone.SoundData = this.SoundData;
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

