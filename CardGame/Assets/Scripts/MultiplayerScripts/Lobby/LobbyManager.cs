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
        // input verilerininin alýma açýk olduðunu belirtme.
        runnerInstance.ProvideInput = true;                                   // _networkRunner.ProvideInput boolean deðeri bizim serverýmýza baðlanan host veya clientin input deðerlerini server a yollama durumunu tutar. Hareket,efect,phsicsvb.
                                                                              // true ise bu kullanýcýnýn verilerini server a yollama iþlemi yaptýðý esnada serverýn bunlarý kabul etmesine yarar. 
                                                                              // Sahne referansý alma ve bu sahneyi networksceneýnfo içerisine atma.   
        var _sceneRef = SceneRef.FromIndex(1); //  SceneRef classý bir sahnenin bilgilerini içeren bir classtýr.Fromindex ile sahnenin indexi ne ise o sahnenin bilgilerini
                                               // içerir.
        var _sceneInfo = new NetworkSceneInfo(); // Bu class network üzerinden sahneler arasý geçiþ vb. iþlemlerin yapýldýðý classtýr. 

        if (_sceneRef.IsValid) // SceneRef deðeri var ise 
        {
            _sceneInfo.AddSceneRef(_sceneRef, LoadSceneMode.Additive);  //LoadSceneMode.Additive  Mevcut Sahnenin && üzerine && bu sahneyi ekler, AddSceneRef ise bu referans ý bünyesinde barýndýrýr.
        }
        // Oturum Baþlatma 
        await runnerInstance.StartGame(new StartGameArgs()    // nrtworkrunner.startGame ile oyun içerisine giriyoruz modu oturumun ismini yüklenecek sahneyi scene manager ý deðerlerini StartGameArgs() içerisinde veriyoruz.
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
