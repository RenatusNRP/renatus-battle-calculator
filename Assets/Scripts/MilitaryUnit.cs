using Unity.Netcode;
using UnityEngine;

public class MilitaryUnit : NetworkBehaviour
{
    public NetworkVariable<NetworkObjectReference> owningPlayer;
    
}

