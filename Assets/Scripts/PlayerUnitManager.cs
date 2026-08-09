using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerUnitManager : NetworkBehaviour
{
    public GameObject unitPrefab;
    public NetworkList<NetworkObjectReference> troops;
    [Rpc(SendTo.Server)]
    void SpawnUnitRequestForMeRpc(int x, int y, RpcParams rpcParams)
    {
        
        var unit = Instantiate(unitPrefab);
        unitPrefab.transform.position = new Vector3(x, y, 0);
        unit.GetComponent<NetworkObject>().Spawn();
        unit.GetComponent<MilitaryUnit>().owningPlayer.Value = new NetworkObjectReference(NetworkManager.Singleton.ConnectedClients[rpcParams.Receive.SenderClientId].PlayerObject);

        NetworkManager.Singleton.ConnectedClients[rpcParams.Receive.SenderClientId].PlayerObject.GetComponent<PlayerUnitManager>().troops.Add(unit);
        UnitSpawnedRpc(new NetworkObjectReference(unit.GetComponent<NetworkObject>()), RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));
    }
    [Rpc(SendTo.SpecifiedInParams)]
    void UnitSpawnedRpc(NetworkObjectReference spawnedUnit, RpcParams rpcParams)
    {
        Debug.Log("Troop Spawned!!!");
    }
    public void RequestUnit(int x, int y)
    {
        SpawnUnitRequestForMeRpc(x, y, new RpcParams());
    }
}
