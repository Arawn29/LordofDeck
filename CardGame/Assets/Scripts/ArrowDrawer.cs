using Fusion;
using System;
using UnityEngine;

public class ArrowDrawer : NetworkBehaviour
{
    public static ArrowDrawer Instance { get; private set; }
    [Networked] public Vector3 StartPosition { get; set; }
    [Networked] public Vector3 EndPosition { get; set; }
    [Networked] public NetworkObject Arrow { get; set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void SpawnArrow(PlayerRef player)
    {
        if (!Runner.IsServer) return;
        Arrow = Runner.Spawn(PrefabManager.instance.ArrowDrawerPrefab, Vector3.zero, Quaternion.identity, player);

    }
    
    public void SetPositions(Vector3 startPosition, Vector3 endPosition)
    {
        Arrow.transform.position = startPosition;
        LineRenderer lineRenderer = Arrow.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        EndPosition = endPosition;
        StartPosition = startPosition;
        RPC_SyncPositions();
        
    }
    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    private void RPC_SyncPositions()
    {
        LineRenderer lineRenderer = Arrow.gameObject.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, StartPosition);
        lineRenderer.SetPosition(1, EndPosition);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DrawArrow()
    {
        // Creating Arrow
        LineRenderer lineRenderer = Arrow.GetComponent<LineRenderer>();
        Arrow.transform.position = lineRenderer.GetPosition(1);
        Vector3 direction = new Vector3(EndPosition.x - StartPosition.x, EndPosition.y - Camera.main.transform.position.y, EndPosition.z - StartPosition.z);
        Arrow.transform.rotation = Quaternion.LookRotation(direction);

    }
    public void RemoveArrow()
    {
        Runner.Despawn(Arrow);
    }

}