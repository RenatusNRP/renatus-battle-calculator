using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if NEW_INPUT_SYSTEM_INSTALLED
using UnityEngine.InputSystem.UI;
#endif


    /// <summary>
    /// A basic example of a UI to start a host or client.
    /// If you want to modify this Script please copy it into your own project and add it to your copied UI Prefab.
    /// </summary>
public class TemporaryUI : MonoBehaviour
{
    [SerializeField]
    Button m_StartHostButton;
    [SerializeField]
    Button m_StartClientButton;
    [SerializeField]
    Button m_SpawnTroopButton;

    bool spawnTroopOnNextClick = false;
    Camera cam;

    void Awake()
    {
        if (!FindAnyObjectByType<EventSystem>())
        {
            var inputType = typeof(StandaloneInputModule);
#if ENABLE_INPUT_SYSTEM && NEW_INPUT_SYSTEM_INSTALLED
            inputType = typeof(InputSystemUIInputModule);                
#endif
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), inputType);
            eventSystem.transform.SetParent(transform);
        }
        cam = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_StartHostButton.onClick.AddListener(StartHost);
        m_StartClientButton.onClick.AddListener(StartClient);
        m_SpawnTroopButton.onClick.AddListener(SpawnTroop);
    }

    void Update()
    {
        if(spawnTroopOnNextClick && Input.GetMouseButtonDown(0))
        {
            spawnTroopOnNextClick = false;

            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z += 10;
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnitManager>().RequestUnit(pos);

        }

    }

    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        DeactivateButtons();
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        DeactivateButtons();
    }

    void SpawnTroop()
    {
        spawnTroopOnNextClick = true;
    }



    void DeactivateButtons()
    {
        m_StartHostButton.interactable = false;
        m_StartClientButton.interactable = false;
    }
}
