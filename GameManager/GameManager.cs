using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEngine.InputSystem;
//Could use this to warp mouse position to the spell bar...

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //Player
    [SerializeField] private GameObject playerPrefab;
    [HideInInspector] public GameObject player;

    [SerializeField] private Vector3 playerStartingPosition;

    private void Awake()
    {
        //SINGLETON
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        //ACTIONS
        Actions.OnSpellPanelToggle += HandleSpellPanelToggle;

    }

    private void Start()
    {
        // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        //QualitySettings.vSyncCount = 0; 

        // Limit framerate
        //Application.targetFrameRate = 30;

        player = GameObject.Instantiate(playerPrefab, playerStartingPosition, Quaternion.Euler(Vector3.zero), null);

        //Mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandleSpellPanelToggle(bool isOpen)
    {
        if (isOpen)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
