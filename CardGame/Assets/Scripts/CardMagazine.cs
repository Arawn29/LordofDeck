using System.Collections.Generic;
using UnityEngine;

public class CardMagazine : MonoBehaviour
{
    public static CardMagazine instance;

    //CardData bir SO olup attack,defans,heal,speacial gibi özel kart türleri vardýr.
    public List<CardData> AllCards = new List<CardData>();

    // CardFrame ise bu kart türlerinin çerçevelerinin Prefab þeklini tutar, CardType enum a göre Prefab belirlenir.
    [SerializeField] public SerializedDictioanry<CardType, GameObject> CardFrame = new SerializedDictioanry<CardType, GameObject>();

    [SerializeField] public SerializedDictioanry<CardType, Sprite> CardSymbols = new SerializedDictioanry<CardType, Sprite> ();

    private void Awake()
    {

        if (instance == null)
            instance = this;
        else
        {
            Destroy(instance);
        }

    }
}

