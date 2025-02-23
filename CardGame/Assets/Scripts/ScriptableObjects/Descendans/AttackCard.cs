using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Attack Card", menuName = "Add Card/Attack Card/Normal Attack Card")]

public class AttackCard : CardData
{
    [SerializeField] public int Damage;
    [SerializeField] public int Health;
    [SerializeField] public int ArmorCount;


    private void Awake()
    {
        CardType = CardType.AttackCard;
        CardTarget = EffectRange.Single;


    }
    public override string[] GetCardValuesString()
    {
        return new string[] { Damage.ToString(), Health.ToString(), ArmorCount.ToString() };
    }
    public override string[] GetCardUIValues()
    {
        return new string[] { ManaCost.ToString(), Damage.ToString(), Health.ToString() };
    }

    public override void CardEFfect(object Enemycard)
    {
        if (Enemycard is Card card)
        {
            AttackCard enemyCard = card._CardData as AttackCard;
            if (enemyCard != null)
            {
                #region ThisCardProcedure
                Card thisCard = GameManager.instance.GetACardofPlayer(Owner,this);
                if (thisCard.IsFrontly == false)
                {
                    thisCard.IsFrontly = true;
                }
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

                if (enemyCard.Health <= 0 && GameManager.instance.IsItServer())
                {
                    card.DestroyCard();
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
        AttackCard clone = CreateInstance<AttackCard>();

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
