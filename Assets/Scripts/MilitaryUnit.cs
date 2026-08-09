using Unity.Netcode;
using UnityEngine;

public class MilitaryUnit : NetworkBehaviour
{
    public NetworkVariable<NetworkObjectReference> owningPlayer;
    public void Update()
    {

        if (owningPlayer.Value.TryGet(out NetworkObject owner) && owner == NetworkManager.Singleton.LocalClient.PlayerObject)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else gameObject.GetComponent<SpriteRenderer>().color = Color.red;
    }
}

