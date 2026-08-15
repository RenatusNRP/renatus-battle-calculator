using System;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
#if NEW_INPUT_SYSTEM_INSTALLED
using UnityEngine.InputSystem.UI;
#endif

[RequireComponent(typeof(RelayManager))]
public class UIAndCamera : MonoBehaviour
{
    [SerializeField]
    Button m_StartHostButton;
    [SerializeField]
    Button m_StartClientButton;
    [SerializeField]
    Button m_SpawnTroopButton;

    bool spawnTroopOnNextClick = false;
    Camera cam;
    public const float EDGE_THRESHOLD = 0.45f;
    public float speed = 10f;
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
        m_SpawnTroopButton.gameObject.SetActive(false);
    }

    

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenUV = new(mousePos.x / Screen.width - 0.5f, mousePos.y / Screen.height - 0.5f);
        Vector3 move = Vector3.zero;
        bool moveLeft = false, moveRight = false, moveUp = false, moveDown = false;
        if (Input.GetMouseButton(1))
        {
            if (screenUV.x < -EDGE_THRESHOLD)
            {
                moveLeft = true;
            }
            if (screenUV.x > EDGE_THRESHOLD)
            {
                moveRight = true;
            }
            if (screenUV.y < -EDGE_THRESHOLD)
            {
                moveDown = true;
            }
            if (screenUV.y > EDGE_THRESHOLD)
            {
                moveUp = true;
            }
        }

        moveLeft = moveLeft || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        moveRight = moveRight || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);
        moveDown = moveDown || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        moveUp = moveUp || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);


        if (moveLeft)
        {
            move.x = -1f;
        }
        if (moveRight)
        {
            move.x = 1f;
        }
        if (moveDown)
        {
            move.y = -1f;
        }
        if (moveUp)
        {
            move.y = 1f;
        }

        move = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * move.normalized;
        cam.transform.Translate(move * (Time.deltaTime * speed), Space.World);

        if (spawnTroopOnNextClick && Input.GetMouseButtonDown(0))
        {
            spawnTroopOnNextClick = false;
            Vector3 pos = cam.ScreenToWorldPoint(mousePos);
            pos.z += 10;
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerUnitManager>().RequestUnit(pos);
        }

    }

    void StartClient()
    {
        DeactivateButtons();
        GetComponent<RelayManager>().JoinRelay();
    }

    void StartHost()
    {
        DeactivateButtons();
        GetComponent<RelayManager>().StartRelay();
    }



    void SpawnTroop()
    {
        spawnTroopOnNextClick = true;
    }



    public void DeactivateButtons()
    {
        //m_StartHostButton.interactable = false;
        //m_StartClientButton.interactable = false;
        m_StartHostButton.gameObject.SetActive(false);
        m_StartClientButton.gameObject.SetActive(false);
    }
    public void ReactivateButtons()
    {
        m_StartHostButton.gameObject.SetActive(true);
        m_StartClientButton.gameObject.SetActive(true);
    }
    public void EnableInGameInteractions()
    { 
        m_StartClientButton.gameObject.SetActive(true);
    }
}
