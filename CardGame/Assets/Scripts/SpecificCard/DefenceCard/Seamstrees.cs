
using Fusion;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defence Card", menuName = "Add Card/Defence Card / SeamStrees")]
public class Seamstrees : DefenceCard,IDespawned

{
    private void Awake()
    {
        CardType = CardType.DefenceCard;
        CardTarget = EffectRange.Single;

    }
    private void OnEnable()
    {
        TurnManager.OnRoundChanged += OnRoundChanged;

    }

    public override void CardEFfect(object AllyCard)
    {
      
    }
    public void OnRoundChanged(PlayerRef player)
    {
        if (GameManager.instance.GetACardofPlayer(Owner, this) == null)
        {
            TurnManager.OnRoundChanged -= OnRoundChanged;
            return;
        }
        if (player == Owner && GameManager.instance.GetACardofPlayer(Owner, this).IsFrontly)
        {
            List<Card> attackCards = GameManager.instance.GetSpecificCardsofPlayer(CardType.AttackCard,player) ;
            Card thisCard =null;

            if (thisCard != null)
            {
                NetworkObject thiscardObject = thisCard.GetComponent<NetworkObject>();  
                if (thiscardObject != null) 
                {
                    CardManager.Instance.RPC_PlaySound((int)ClipType.GiveEffect,thiscardObject);
                }
            }
            if (attackCards.Count < 1) return;
            int random = Random.Range(0, attackCards.Count);
            NetworkObject targetAllie = attackCards[random].GetComponent<NetworkObject>();
            Card targetcard = targetAllie.GetComponent<Card>();
            if (targetcard != null)
            {
                if (targetcard._CardData is AttackCard attackCard)
                {
                    attackCard.ArmorCount += ArmorCount;
                    Debug.Log($"{targetcard.name} hedef ");
                    targetcard.UpgradeCardUnits();
                }
            }

        }
    }
    public override CardData Clone()
    {
        Seamstrees clone = CreateInstance<Seamstrees>();
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

    public void Despawned(NetworkRunner runner, bool hasState)
    {
        

    }
}
