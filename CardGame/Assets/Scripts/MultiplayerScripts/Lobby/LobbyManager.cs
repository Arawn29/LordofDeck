using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static LobbyManager Instance;
    public static NetworkRunner runnerInstance;
    public string sessionName = "default";
    public GameObject SessionPrefab;

    public GameObject ScrollViewContent;
    public TextMeshProUGUI RoomNameFromText;
    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else Destroy(this);
        CreateRunner();
        
    }
    
    private void CreateRunner()
    {
        runnerInstance = GetComponent<NetworkRunner>();
        if (runnerInstance == null) { runnerInstance = gameObject.AddComponent<NetworkRunner>(); }
    }
    public void SetSessionName()
    {
        sessionName = RoomNameFromText.text;
    }
    public void Start()
    {
        runnerInstance.JoinSessionLobby(SessionLobby.Shared, sessionName);
    }
    public void CreateRoomButton()
    {
        GameStarted();
    }
    public async void GameStarted()
    {

        if (sessionName == "" || sessionName == null) { return; }
        //runnerInstance = gameObject.AddComponent<NetworkRunner>();
        // input verilerininin al�ma a��k oldu�unu belirtme.
        runnerInstance.ProvideInput = true;                                   // _networkRunner.ProvideInput boolean de�eri bizim server�m�za ba�lanan host veya clientin input de�erlerini server a yollama durumunu tutar. Hareket,efect,phsicsvb.
                                                                              // true ise bu kullan�c�n�n verilerini server a yollama i�lemi yapt��� esnada server�n bunlar� kabul etmesine yarar. 
                                                                              // Sahne referans� alma ve bu sahneyi networkscene�nfo i�erisine atma.   
        var _sceneRef = SceneRef.FromIndex(1); //  SceneRef class� bir sahnenin bilgilerini i�eren bir classt�r.Fromindex ile sahnenin indexi ne ise o sahnenin bilgilerini
                                               // i�erir.
        var _sceneInfo = new NetworkSceneInfo(); // Bu class network �zerinden sahneler aras� ge�i� vb. i�lemlerin yap�ld��� classt�r. 

        if (_sceneRef.IsValid) // SceneRef de�eri var ise 
        {
            _sceneInfo.AddSceneRef(_sceneRef, LoadSceneMode.Additive);  //LoadSceneMode.Additive  Mevcut Sahnenin && �zerine && bu sahneyi ekler, AddSceneRef ise bu referans � b�nyesinde bar�nd�r�r.
        }
        // Oturum Ba�latma 
        await runnerInstance.StartGame(new StartGameArgs()    // nrtworkrunner.startGame ile oyun i�erisine giriyoruz modu oturumun ismini y�klenecek sahneyi scene manager � de�erlerini StartGameArgs() i�erisinde veriyoruz.
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = sessionName,
            Scene = _sceneRef,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 2,
        }
        );


    }

    public async void ShutDown()
    {
     await  runnerInstance.Shutdown();
    }
    #region RunnerCalls

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    => GameManager.instance.GameWinner();


    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

        
        MultiplayerSpawner instance = MultiplayerSpawner.Instance;
        if (instance != null)
        {
            instance.SpawnPlayerObjects(runner, player);
            instance.AddPlayerToList(player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
      
   
        Debug.LogWarning($"{player} gitti ");
        
        foreach (var _player in MultiplayerSpawner.Instance.PlayersInScene)
        {
            if (_player != player)
            {
                // Kazanan
                GameManager.instance.GameWinner();
            }

        }
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        foreach (Transform child in ScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var session in sessionList)
        {
            GameObject roomInstance = Instantiate(SessionPrefab);
            roomInstance.transform.SetParent(ScrollViewContent.transform);
            SessionInstance data = roomInstance.transform.GetComponent<SessionInstance>();
            data.SessionName = session.Name;
            data.SessionType = "Classic";   // For other modes
            data.SessionCapacity = $"{session.PlayerCount} / {session.MaxPlayers}";
            data.UpdateProperties();
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
    #endregion
}
