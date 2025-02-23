using Fusion;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : NetworkBehaviour
{
    public TextMeshPro[] cardTexts = new TextMeshPro[3];
    public Sprite Sprite;
    public Animator animator;
    #region Networked Parameters
    [Networked] public PlayerRef CardPlayer { get; set; }
    [Networked] public int CardID { get; set; }

    [Networked] public int CardType { get; set; }

    [Networked] public bool IsFrontly { get; set; }
    #endregion
    [Networked] public bool isCardHasPlayed { get; set; }
    public CardData _CardData;

    [Networked] public NetworkObject _ParentNetworkObject { get; set; }

    [Networked] public Vector3 StartPosition { get; set; }
    [Networked] public bool isArrowDrawing { get; set; } = false;
    [Networked] public bool isCardMoving { get; set; } = false;
    [Networked] public bool isCardAttacking { get; set; } = false;
    [Networked] public bool isCardEffect { get; set; } = false;
    [Networked] public NetworkObject TargetObject { get; set; }
    [Networked] public Vector3 TargetPosition { get; set; }
    public TickTimer Counter { get; set; }
    private void Start()
    {
        Counter = TickTimer.None;
    }
    public bool isDragging = false;
    float elapsedTime;
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) { return; }
        if (isCardHasPlayed) { return; }

        if (Counter.RemainingTime(Runner) <= 4f && isArrowDrawing)
        {
            CanvaSys.Instance.RPC_ChangeStatTurnButton();
            isArrowDrawing = false;
            CardManager.Instance.isInProccces = true;
            ArrowDrawer.Instance.SetPositions(Object.transform.position, TargetPosition);
            ArrowDrawer.Instance.RPC_DrawArrow();
            isCardMoving = true;
        }
        if (Counter.RemainingTime(Runner) <= 3f && isCardMoving)
        {
            isCardMoving = false;
            ArrowDrawer.Instance.RemoveArrow();
            CardManager.Instance.RPC_PlaySound((int)ClipType.Move, Object);
            StartCoroutine(MoveManager.instance.CardNetworkMove(Object, TargetPosition, 1f, 0.8f));
            isCardAttacking = true;


        }

        if (Counter.RemainingTime(Runner) <= 1.8f && isCardAttacking)
        {
            CardManager.Instance.RPC_StopSound();
            isCardAttacking = false;
            animator.Play("CardAttack");
            isCardEffect = true;
            CardManager.Instance.RPC_PlaySound((int)ClipType.GiveEffect, Object);

        }


        if (Counter.RemainingTime(Runner) <= 1.4f && isCardEffect)
        {
            isCardEffect = false;
            CardManager.Instance.RPC_StopSound();
            animator.Play("CardIdle");
            StartCoroutine(MoveManager.instance.CardNetworkMove(Object, StartPosition, 0.2f, 1f));
            if (TargetObject != null)
            {
                if (TargetObject.GetComponent<Card>() != null)
                {
                    if (_CardData is AttackCard)
                    {
                        CardManager.Instance.RPC_PlaySound((int)ClipType.TakeEffect, TargetObject);
                    }
                }
            }
            RPC_PlayCard(TargetObject, gameObject.GetComponent<NetworkObject>());
            CanvaSys.Instance.RPC_ChangeStatTurnButton();
            Debug.Log("Þuan burada ve sayým bitti");
            CardManager.Instance.isInProccces = false;

            Counter = TickTimer.None;

        }
    }
    public void Inýtializer(CardData cardData, PlayerRef player, NetworkObject Parent, bool _isFrontly)   //OnlyServer
    {
        //OnlyServer
        #region CardMatching
        CardID = cardData.CardID;
        CardType = (int)cardData.CardType;
        IsFrontly = _isFrontly;
        Sprite = cardData.sprite;
        CardPlayer = player;
        #endregion                
        _ParentNetworkObject = Parent;
        CardManager.Instance.RPC_CardInitialize(this.Object);
    }

    public void OnMouseDrag()
    {
        elapsedTime += Time.deltaTime;

        Debug.Log("there");
        if (elapsedTime >= 0.1f)
        {
            isDragging = true;
        }
    }
    public void OnMouseUp()
    {
        elapsedTime = 0f;
        if (isDragging)
        {
            isDragging = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 750) && !isCardHasPlayed && Runner.LocalPlayer == TurnManager.instance.CurrentTurnOwner() && CardPlayer == Runner.LocalPlayer && TurnManager.instance.RoundCount != 1)
            {
                if (hit.transform.TryGetComponent<NetworkObject>(out var hittedNetworkObject) && !CardManager.Instance.isInProccces)
                {
                    Debug.Log(hittedNetworkObject.name);
                    RPC_CheckPlayability(hittedNetworkObject, this.gameObject.GetComponent<NetworkObject>());
                }
            }
            return;
        }
        if (!isDragging)
        {
            if (Input.GetMouseButtonUp(0))
            {
                CanvaSys.Instance.SetInfoPanProp(this);
            }
            if (Input.GetMouseButtonUp(1))
            {
                CanvaSys.Instance.ResetInfoPanProp();
            }
        }

    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_CheckPlayability(NetworkObject hittedObject, NetworkObject hitterObject)
    {
        TargetObject = null;
        Card hitterCard = hitterObject.GetComponent<Card>();
        Card hittedCard = hittedObject.GetComponent<Card>();

        if (hitterCard != null && hittedCard == null)
        {
            if (hitterObject.InputAuthority != hittedObject.InputAuthority)
            {
                List<Card> _enemycard = GameManager.instance.GetSpecificCardsofPlayer(global::CardType.AttackCard, hittedObject.InputAuthority);
                switch (hitterCard.CardType)
                {
                    case (int)global::CardType.AttackCard when _enemycard.Count == 0:
                        Counter = TickTimer.CreateFromSeconds(Runner, 4f);
                        ArrowDrawer.Instance.SpawnArrow(hitterObject.InputAuthority);

                        isArrowDrawing = true;
                        TargetObject = hittedObject;
                        TargetPosition = MultiplayerSpawner.Instance.PlayerLocations[hittedObject.InputAuthority];

                        break;

                }
            }   // Attack life Point

        }
        if (hitterCard != null && hittedCard != null)
        {
            if (hitterObject.InputAuthority == hittedObject.InputAuthority)
            {
                switch (hitterCard.CardType)
                {
                    case (int)global::CardType.HealingCard when hittedCard.CardType == (int)global::CardType.AttackCard:

                        if (hitterCard._CardData as OldWoman)
                        {
                            return;
                        }
                        Counter = TickTimer.CreateFromSeconds(Runner, 4f);
                        ArrowDrawer.Instance.SpawnArrow(hitterObject.InputAuthority);
                        isArrowDrawing = true;
                        TargetObject = hittedObject;
                        TargetPosition = hittedObject.transform.position;

                        break;

                    case (int)global::CardType.DefenceCard when hittedCard.CardType == (int)global::CardType.AttackCard:

                        if (hitterCard._CardData as Seamstrees)
                        {
                            return;

                        }
                        Counter = TickTimer.CreateFromSeconds(Runner, 4f);
                        ArrowDrawer.Instance.SpawnArrow(hitterObject.InputAuthority);
                        isArrowDrawing = true;
                        TargetObject = hittedObject;
                        TargetPosition = hittedObject.transform.position;

                        break;

                }
            }     // PowerUp
            else if (hitterObject.InputAuthority != hittedObject.InputAuthority)
            {

                switch (hitterCard.CardType)
                {
                    case (int)global::CardType.AttackCard when hittedCard.CardType == (int)global::CardType.AttackCard:
                        Counter = TickTimer.CreateFromSeconds(Runner, 4f);
                        ArrowDrawer.Instance.SpawnArrow(hitterObject.InputAuthority);

                        isArrowDrawing = true;
                        TargetObject = hittedObject;
                        TargetPosition = hittedObject.transform.position;

                        break;

                }
            } // Attack

        }
        if (hitterCard._CardData is SpecialEffectCard specialEffect)               // Special Effects
        {
            switch (specialEffect)
            {
                case DeSpell:
                    if (hittedCard != null && hittedCard._CardData is not AttackCard && hittedCard.CardPlayer != CardPlayer)
                    {
                        Counter = TickTimer.CreateFromSeconds(Runner, 4f);
                        ArrowDrawer.Instance.SpawnArrow(hitterObject.InputAuthority);
                        isArrowDrawing = true;
                        TargetObject = hittedObject;
                        TargetPosition = hittedObject.transform.position;
                    }
                    break;
                case Courage:
                    if (hittedCard != null && hittedCard._CardData is AttackCard && hittedCard.CardPlayer == hitterCard.CardPlayer)
                    {
                        Counter = TickTimer.CreateFromSeconds(Runner, 4f);
                        ArrowDrawer.Instance.SpawnArrow(hitterObject.InputAuthority);
                        isArrowDrawing = true;
                        TargetObject = hittedObject;
                        TargetPosition = hittedObject.transform.position;
                    }
                    break;
                case Ragnarok:
                    if (GameManager.instance.GetSpecificCardsofPlayer(global::CardType.AttackCard, hittedObject.InputAuthority).Count != 0 || GameManager.instance.GetSpecificCardsofPlayer(global::CardType.AttackCard, hitterObject.InputAuthority).Count != 0)
                    {
                        Counter = TickTimer.CreateFromSeconds(Runner, 4f);
                        ArrowDrawer.Instance.SpawnArrow(hitterObject.InputAuthority);
                        isArrowDrawing = true;
                        TargetObject = hittedObject;
                        TargetPosition = hittedObject.transform.position;

                    }
                    break;
                case BlackHole:
                    if (hittedObject != null)
                    {
                        Counter = TickTimer.CreateFromSeconds(Runner, 4f);
                        ArrowDrawer.Instance.SpawnArrow(hitterObject.InputAuthority);
                        isArrowDrawing = true;
                        TargetObject = hittedObject;
                        TargetPosition = hittedObject.transform.position;
                    }
                    break;
                case DrawCard:

                    Counter = TickTimer.CreateFromSeconds(Runner, 4f);
                    ArrowDrawer.Instance.SpawnArrow(hitterObject.InputAuthority);
                    isArrowDrawing = true;
                    TargetObject = hittedObject;
                    TargetPosition = MultiplayerSpawner.Instance.PlayerLocations[hitterObject.InputAuthority];



                    break;


            }

        }

    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayCard(NetworkObject hittedObject, NetworkObject hitterObject)
    {
        Card hitterCard = hitterObject == null ? null : hitterObject.GetComponent<Card>();
        Card hittedCard = hittedObject == null ? null : hittedObject.GetComponent<Card>();
        Debug.Log("Buraya geldi ve hittedCard null olma durumu :" + (hittedObject == null ? null : hittedObject.name));
        //Attack Health Value ;
        if (hitterCard != null && hittedCard == null)
        {

            switch (hitterCard.CardType)
            {
                case (int)global::CardType.AttackCard when GameManager.instance.GetSpecificCardsofPlayer(global::CardType.AttackCard, hittedObject.InputAuthority).Count == 0:
                    AttackCard attackCard = hitterCard._CardData as AttackCard;
                    Debug.Log($"{attackCard.Damage} kadar hasar almasý gerekir");
                    HealthManager.instance.RPC_HealthBarChange(hittedObject.InputAuthority, attackCard.Damage);

                    break;
            }
        }

        // Attack/Power UP
        if (hitterCard != null && hittedCard != null)
        {
            //PowerUp
            if (hitterCard.gameObject.GetComponent<NetworkObject>().InputAuthority == hittedCard.GetComponent<NetworkObject>().InputAuthority)
            {
                switch (hittedCard.CardType)
                {
                    case (int)global::CardType.AttackCard when hitterCard.CardType == (int)global::CardType.DefenceCard:
                        if (hitterCard._CardData.CardTarget == EffectRange.Single)
                        {

                            hitterCard._CardData.CardEFfect(hittedCard);

                        }
                        if (hitterCard._CardData.CardTarget == EffectRange.Multiple)
                        {

                            hitterCard._CardData.CardEFfect(GameManager.instance.GetCardsofPlayer(hitterCard.CardPlayer));

                        }
                        break;

                    case (int)global::CardType.AttackCard when hitterCard.CardType == (int)global::CardType.HealingCard:

                        if (hitterCard._CardData.CardTarget == EffectRange.Single)
                        {

                            hitterCard._CardData.CardEFfect(hittedCard);

                        }
                        if (hitterCard._CardData.CardTarget == EffectRange.Multiple)
                        {

                            hitterCard._CardData.CardEFfect(GameManager.instance.GetCardsofPlayer(hitterCard.CardPlayer));


                        }
                        break;

                }
            }



            //Attack
            if (hitterCard.gameObject.GetComponent<NetworkObject>().InputAuthority != hittedCard.GetComponent<NetworkObject>().InputAuthority)
            {
                if (hitterCard._CardData.CardTarget == EffectRange.Single)
                {
                    hitterCard._CardData.CardEFfect(hittedCard);

                }
                if (hitterCard._CardData.CardTarget == EffectRange.Multiple)
                {
                    List<Card> cardList = GameManager.instance.GetCardsofPlayer(hittedCard.CardPlayer);

                    hitterCard._CardData.CardEFfect(cardList); ;

                }
            }

        }

        // Special Effect 
        if (hitterCard._CardData is SpecialEffectCard specialEffect)               // Special Effects
        {
            switch (specialEffect)
            {
                case DeSpell:
                    if (hittedCard != null && hittedCard._CardData is not AttackCard && hittedCard.CardPlayer != CardPlayer)
                    {
                        hitterCard._CardData.CardEFfect(hittedCard);
                    }
                    break;
                case Courage:
                    if (hittedCard != null && hittedCard._CardData is AttackCard && hittedCard.CardPlayer == hitterCard.CardPlayer)
                    {
                        hitterCard._CardData.CardEFfect(hittedCard);
                    }
                    break;
                case Ragnarok:
                    if (hittedObject != null && GameManager.instance.GetSpecificCardsofPlayer(global::CardType.AttackCard, hittedObject.InputAuthority).Count != 0 || GameManager.instance.GetSpecificCardsofPlayer(global::CardType.AttackCard, hitterObject.InputAuthority).Count != 0)
                    {
                        List<Card> allCards = GameManager.instance.GetAllCardsOnScene();
                        List<Card> allAttackCards = new List<Card>();
                        foreach (var item in allCards)
                        {
                            if (item._CardData as AttackCard)
                            {
                                allAttackCards.Add(item);
                            }
                        }
                        hitterCard._CardData.CardEFfect(allAttackCards);
                    }
                    break;
                case BlackHole:
                    if (hittedObject != null)
                    {

                        hitterCard._CardData.CardEFfect(GameManager.instance.GetAllCardsOnScene());
                    }
                    break;
                case DrawCard:
                    hitterCard._CardData.CardEFfect(Runner.LocalPlayer);
                    break;

            }

        }

    }
    public void UpgradeCardUnits()
    {
        CardManager.Instance.CardUpdateSync(this.Object);
    }
    public void DestroyCard()
    {
        DespawnObject();
        RPC_DestroyCardSync();
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DestroyCardSync()
    {
        if (GameManager.instance.PlayersCards.TryGetValue(CardPlayer, out List<NetworkObject> cardlist))
        {

            cardlist?.Remove(this.Object);

            GetComponent<BoxCollider>().enabled = false;
            CardArea area = _ParentNetworkObject?.transform.GetComponent<CardArea>();
            if (area != null)
            {
                area.isEmpty = true;
            }


        }
    }
    public override void Spawned()
    {
        StartPosition = Object.transform.position;
    }
    private async void DespawnObject()
    {
        if (Runner.IsServer)
        {
            await Task.Delay(1000);
            Runner.Despawn(Object);
        }
    }
}

