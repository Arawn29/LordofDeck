using Fusion;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class GameTimer : NetworkBehaviour

{
    GameObject EndTurnPan;
    public GameObject _timerGameObj;
    public TickTimer timer { get; set; }


    bool isPlayersReady = false;
    bool isGameStarted = false;


    private void Awake()
    {
        _timerGameObj.SetActive(false);
        EndTurnPan = CanvaSys.Instance.TurnButton;
        EndTurnPan.SetActive(false);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;
        if (Runner.ActivePlayers.Count() == 2 && !isPlayersReady)
        {
            isPlayersReady = true;

            GameManager.instance.GetActivePlayersInGame();
            StartTimer(4f);
            RPC_TimerProcces();
        }


        if (timer.IsRunning)
        {
            float _remainingTime = timer.RemainingTime(Runner) ?? 0;

            RPC_TimerTextSyncs(Mathf.FloorToInt(_remainingTime));

        }

        if (timer.Expired(Runner) && !isGameStarted)
        {
            isGameStarted = true;

            RPC_GameStarting();


        }

    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_GameStarting()
    {

        _timerGameObj.SetActive(false);
        CardInventory.Instance.CreatingInventory();


        if (Runner.IsServer)
        {
            TurnManager.instance.Initializer();
        }
        for (int i = 0; i < MultiplayerSpawner.Instance.PlayersInScene.Count; i++)
        {
            HealthManager.instance.SpawnBar(MultiplayerSpawner.Instance.PlayersInScene[i]);
        }
        EndTurnPan.SetActive(true);

    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_TimerProcces()
    {
        _timerGameObj.SetActive(true);
        GameManager.instance.RPC_CardListCreating();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_TimerTextSyncs(int duration)
    {
        foreach (TextMeshProUGUI text in _timerGameObj.GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.text = duration.ToString();
        }
    }

    private void StartTimer(float duration)

    {
        timer = TickTimer.CreateFromSeconds(Runner, duration);

    }

}
