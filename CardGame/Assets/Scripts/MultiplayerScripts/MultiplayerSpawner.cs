using Fusion;
using System.Collections.Generic;
using UnityEngine;


public class MultiplayerSpawner : MonoBehaviour
{
    public static MultiplayerSpawner Instance;
    NetworkPrefabRef PlayerPrefab;
    NetworkPrefabRef CardPlayAreaPrefab;
    [SerializeField] private Transform[] CameraPositions = new Transform[2];
    [SerializeField] private Transform[] PlayerSpawnPoints = new Transform[2];
    [SerializeField] private Transform[] CardPlayAreaPoints = new Transform[2];
    public List<PlayerRef> PlayersInScene = new List<PlayerRef>();
    public Dictionary<PlayerRef, Vector3> PlayerLocations = new Dictionary<PlayerRef, Vector3>();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
        PlayerPrefab = PrefabManager.instance.PlayerPrefab;
        CardPlayAreaPrefab = PrefabManager.instance.CardPlayAreaPrefab;
    }
    public void SpawnPlayerObjects(NetworkRunner runner, PlayerRef player)

    {
        UnityEngine.Debug.Log("Þuanda tamda burada dadadada");
        int _index = GetSpawnIndexForPlayer(player);

        if (runner.IsServer)

        {
            NetworkObject PlayerNetworkedObj = runner.Spawn(PlayerPrefab, PlayerSpawnPoints[_index].position, PlayerSpawnPoints[_index].rotation, player);
            RPC_SetPlayerLocations(player, PlayerSpawnPoints[_index].position);
            NetworkObject CardAreaNetworkedObj = runner.Spawn(CardPlayAreaPrefab, CardPlayAreaPoints[_index].position, CardPlayAreaPoints[_index].rotation, player);
            runner.SetPlayerObject(player, CardAreaNetworkedObj);

        }
        if (runner.LocalPlayer == player)

        {
            Camera.main.gameObject.GetComponent<CameraSetup>().SetCameraPosition(CameraPositions[_index]);
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetPlayerLocations(PlayerRef player, Vector3 position)
    {
        if (!PlayerLocations.ContainsKey(player))
        {
            PlayerLocations.Add(player, position);
        }
    }
    private int GetSpawnIndexForPlayer(PlayerRef player)
    {

        return (int)player.PlayerId % PlayerSpawnPoints.Length;
    }
    public void AddPlayerToList(PlayerRef player)
    {
        if (!PlayersInScene.Contains(player))
        {
            PlayersInScene.Add(player);
        }
    }

}
