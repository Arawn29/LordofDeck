using UnityEngine;
using TMPro;
public class SessionInstance : MonoBehaviour
{
    public string SessionName;
    public string SessionType;
    public string SessionCapacity;

    public TextMeshProUGUI NameText;
    public TextMeshProUGUI SessionTypeText;
    public TextMeshProUGUI SessionCapacityText;


    public void OnButtonClicked()
    {
        LobbyManager.Instance.sessionName = SessionName;
        LobbyManager.Instance.GameStarted();
    }
    public void UpdateProperties()
    {
        NameText.text = SessionName;
        SessionTypeText.text = SessionType;
        SessionCapacityText.text = SessionCapacity;
    }
}
