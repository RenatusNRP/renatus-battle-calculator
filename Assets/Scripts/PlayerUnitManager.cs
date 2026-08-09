using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerUnitManager : NetworkBehaviour
{
    public GameObject unitPrefab;
    public NetworkList<NetworkObjectReference> troops;
    [Rpc(SendTo.Server)]
    void SpawnUnitRequestForMeRpc(RpcParams rpcParams)
    {
        var unit = Instantiate(unitPrefab);
        unit.GetComponent<NetworkObject>().Spawn();
        unit.GetComponent<MilitaryUnit>().owningPlayer.Value = new NetworkObjectReference(NetworkManager.Singleton.ConnectedClients[rpcParams.Receive.SenderClientId].PlayerObject);
        UnitSpawnedRpc(new NetworkObjectReference(unit.GetComponent<NetworkObject>()), RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));
    }
    [Rpc(SendTo.SpecifiedInParams)]
    void UnitSpawnedRpc(NetworkObjectReference spawnedUnit, RpcParams rpcParams)
    {
        troops.Add(spawnedUnit);
        Debug.Log("Troop Spawned!!!");
    }
    public void RequestUnit()
    {
        SpawnUnitRequestForMeRpc(new RpcParams());
    }
}
