using Fusion;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof (AudioSource))]
public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IEndDragHandler
{
    public static bool isCardMoving = true; // Cart hareket halinde mi yani dizim aþamasýnda mý bu bool CanvaSys de de set edilmekte.
    static bool isCardClicked = false; // Kart týklama iþlemi gerçekleþti mi 
    public bool isCardDragging = false;
    MoveManager moveManager;

    [HideInInspector]
    public Vector3 _firstPosition;

    private Coroutine IndicationCoroutine; // Kartýn üzerine gelindiðindde kartýn biazcýk yukarý çýkmasý 

    float Indicationmagnitude;  // yukarý cýkma derecesi

    Vector3 _firstRotation;
    Vector3 _firstScale;
    int _firstIndex;
    RectTransform recTransform;

    //-------------------
    AudioSource audioSource;
    //-------------------
    [Header("Card Ýnfo")]
    [HideInInspector]
    public CardData CardData;
    public int CardUniqueID;
    private void Awake()
    {
        recTransform = GetComponent<RectTransform>();
        moveManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<MoveManager>();
        Indicationmagnitude = 100f;
        _firstScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();

    }
    private void Start()
    {
        SoundManager.Instance.PlaySound("CardUIDraw");
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Space))
        {
            isCardClicked = false;
            CanvaSys.Instance.ResetInfoPanProp();
            ResetTransformofCard();
        }
    }
    #region Pointer Events
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isCardClicked) { return; }
        if (isCardMoving) { return; }
        if (isCardDragging) { return; }
        if (IndicationCoroutine != null)
            StopCoroutine(IndicationCoroutine);

        isCardClicked = true;

        CanvaSys.Instance.SetInfoPanProp(this);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isCardClicked) { return; }
        if (isCardMoving) { return; } // Kart Dizilirken pointer çalýþmasýn

        transform.SetAsLastSibling();
        Vector3 _lastPosition = transform.position + transform.up * Indicationmagnitude;
        IndicationCoroutine = StartCoroutine(moveManager.CardMovingTo(gameObject, transform.position, _lastPosition, 0.2f));

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isCardMoving) { return; }
        if (isCardClicked) { return; }

        if (IndicationCoroutine != null) StopCoroutine(IndicationCoroutine);

        ResetTransformofCard();

    }

    #endregion
    public void OnDrag(PointerEventData eventData)
    {
        if (isCardMoving) return;
        isCardDragging = true;
        recTransform.anchoredPosition += eventData.delta;

    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isCardMoving) return;


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 250f))
        {
            NetworkObject hittedObj = hit.collider.gameObject.GetComponent<NetworkObject>();

            if (hittedObj != null && hittedObj.HasInputAuthority)
            {
                CardUniqueID = Guid.NewGuid().GetHashCode();
                CardInformations card = new CardInformations()
                {
                    AreaNetworkObj = hittedObj,
                    player = hittedObj.InputAuthority,
                    CardData = CardData,
                    CardUniqeID = CardUniqueID,
                };

                if (TurnManager.IsTurnOwner && ManaManager.instance.CheckPlayability(card.CardData.ManaCost))
                {
                    CardManager.Instance.SetFaceDeciderPan();
                    CardManager.Instance.SetTheCardInfos(card);
                }

            }
            if (gameObject != null)
            {
                ResetTransformofCard();
            }
        }

        isCardDragging = false;
    }

    void ResetTransformofCard()
    {
        transform.rotation = Quaternion.Euler(_firstRotation);
        transform.position = _firstPosition;
        transform.localScale = _firstScale;
        transform.SetSiblingIndex(_firstIndex);

    }

    public void SetTransformofCard(Vector3 pos, Vector3 rot)
    {
        _firstPosition = pos;
        _firstRotation = rot;
        _firstIndex = transform.GetSiblingIndex();
    }
    public void CardPlayed()
    {
       
        CardInventory.Instance.CardsOnHand.Remove(this.gameObject);

        Destroy(gameObject);


    }

    /*   private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.red;

        Gizmos.DrawRay(ray.origin, ray.direction * 250f);
    }
    */
}
