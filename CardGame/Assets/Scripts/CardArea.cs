using Fusion;


public enum CardAreaType
{

    AttackArea = 0,
    OtherArea = 1
}

public class CardArea : NetworkBehaviour
{
    public bool isEmpty = false;
    public CardAreaType CardareaType;

}
