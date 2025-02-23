using Fusion;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Card", menuName = "Add Card/Attack Card/Hecate")]
public class Hecate : AttackCard
{
    public override string[] GetCardValuesString()
    {
        return new string[] { "-", Health.ToString(), ArmorCount.ToString() };
    }
    public override string[] GetCardUIValues()
    {
        return new string[] { ManaCost.ToString(), "-", Health.ToString() };
    }
    private void Awake()
    {
        CardTarget = EffectRange.Single;
    }
    public override void CardEFfect(object Enemycard)
    {
        if (Enemycard is Card _enemycard)
        {
            if (_enemycard._CardData is Hecate)
            {
                return;
            }
            if (_enemycard._CardData is AttackCard _enemyAttackcard)
            {
                _enemyAttackcard.Health = 1;
                _enemyAttackcard.ArmorCount = 0;
                _enemycard.UpgradeCardUnits();

                NetworkObject myAreas = GameManager.instance.GetPlayerAreas(Owner);
                (CardArea, Vector3) TargetArea = CheckArea(myAreas);
                if (TargetArea.Item1 != null)
                {
                    #region EnemyCardProcedure 

                    _enemycard.isCardHasPlayed = false;
                    _enemycard._ParentNetworkObject.GetComponent<CardArea>().isEmpty = true;
                    _enemycard._ParentNetworkObject = TargetArea.Item1.GetComponent<NetworkObject>();
                    _enemycard.StartPosition = TargetArea.Item2;
                    GameManager.instance.RPC_AddCardforPlayerList(Owner, _enemycard.GetComponent<NetworkObject>());
                    GameManager.instance.RPC_RemoveCardforPlayerList(_enemycard.CardPlayer, _enemycard.GetComponent<NetworkObject>());
                    _enemyAttackcard.Owner = Owner;
                    _enemycard.CardPlayer = Owner;
                    TargetArea.Item1.isEmpty = false;
                    _enemycard.GetComponent<Animator>().enabled = false;
                    _enemycard.transform.position = TargetArea.Item2;
                    Debug.Log($"Target Position: {TargetArea.Item2}");
                    Debug.Log($"EnemyCard Current Position: {_enemycard.transform.position}");
                    if (_enemycard.IsFrontly == false) _enemycard.IsFrontly = true;
                    if (GameManager.instance.IsItServer() && _enemycard.GetComponent<NetworkObject>().HasStateAuthority)
                    {
                        SetInputAuthority(_enemycard.Object, Owner);
                    }
                    #endregion

                    #region ThisCardProcedure
                    Card thisCard = GameManager.instance.GetACardofPlayer(Owner, this);

                    if (thisCard.IsFrontly == false) { thisCard.IsFrontly = true; }
                    thisCard.isCardHasPlayed = true;
                    thisCard.UpgradeCardUnits();
                    #endregion
                }

            }
        }
    }
    public async void SetInputAuthority(NetworkObject targetObject, PlayerRef player)
    {
        await Task.Delay(1000);
        targetObject.AssignInputAuthority(player);

    }
    public (CardArea, Vector3) CheckArea(NetworkObject targetAreaParent)
    {
        if (targetAreaParent != null)
        {
            CardArea[] _marea = targetAreaParent.gameObject.GetComponentsInChildren<CardArea>();
            foreach (CardArea item in _marea)
            {
                if (item.CardareaType == CardAreaType.AttackArea && item.isEmpty == true)
                {
                    CardArea TargetArea = item;

                    return (TargetArea, item.transform.position);
                }
            }
        }
        return (null, Vector3.zero);
    }
    public override CardData Clone()
    {
        Hecate clone = CreateInstance<Hecate>();

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
