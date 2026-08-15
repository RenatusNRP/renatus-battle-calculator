using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

[RequireComponent(typeof(UIAndCamera))]
public class RelayManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI joinCodeHost;
    [SerializeField] private TMP_InputField joinCodeClient;
    public const int MAX_CONNECTIONS_DEFAULT = 2;
     // Start is called once before the first execution of Update after the MonoBehaviour is created
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void StartRelay()
    {
        string joinCode = await StartHostWithRelay();

        if (joinCode == null)
        {
            Debug.LogError("Join code creation fail");

            GetComponent<UIAndCamera>().ReactivateButtons();
        }
        else
        {
            joinCodeHost.text = joinCode;
            GetComponent<UIAndCamera>().EnableInGameInteractions();
        }
    }
    public async void JoinRelay()
    {
        bool success = await StartClientWithRelay(joinCodeClient.text);
        if (!success)
        {
            Debug.LogError("Failed join!");
            GetComponent<UIAndCamera>().ReactivateButtons();
        }
        else GetComponent<UIAndCamera>().EnableInGameInteractions();
    }


    private async Task<string> StartHostWithRelay(int maxConnections = MAX_CONNECTIONS_DEFAULT)
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    private async Task<bool> StartClientWithRelay(string joinCode)
    {
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(AllocationUtils.ToRelayServerData(joinAllocation, "dtls"));
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }



}
