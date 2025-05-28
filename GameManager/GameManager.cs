using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//using UnityEngine.InputSystem;
//Could use this to warp mouse position to the spell bar...

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Space(10)]
    [Header("Audio")]
    private AudioListener listener;

    [Space(10)]
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [HideInInspector] public GameObject player;
    [SerializeField] private GameObject magicCameraPrefab;
    [HideInInspector] public GameObject magicCamera;
    [SerializeField] private Vector3 playerStartingPosition;

    [Space(10)]
    [Header("Common Prefabs")]
    public GameObject manaOrbPrefab;
    public GameObject fireBallPrefab;

    [Space(10)]
    [Header("Common Audio Clips")]
    public AudioClip fireBallClip;
    public AudioClip painClip;
    public AudioClip manaCollectClip;

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
    }

    private void Start()
    {
        // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        //QualitySettings.vSyncCount = 0; 

        // Limit framerate
        //Application.targetFrameRate = 30;

        listener = GetComponent<AudioListener>();

        #region FOR TESTING SCENES IN UNITY EDITOR
        //ALLOWS TESTING FROM LEVELS 1+
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            player = GameObject.Instantiate(playerPrefab, playerStartingPosition, Quaternion.Euler(Vector3.zero), null);
            listener.enabled = false; //Deactivate this audio listener just before spawning camera, which has a listener.
            magicCamera = GameObject.Instantiate(magicCameraPrefab, playerStartingPosition, Quaternion.Euler(Vector3.zero), null);
        }
        #endregion
    }

    public void LoadLevel(int level)
    {
        StartCoroutine(GoToSceneAsync(level));
    }

    IEnumerator GoToSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            //print(operation.progress);
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

        //Hide mouse, spawn player
        if (sceneIndex > 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            player = GameObject.Instantiate(playerPrefab, playerStartingPosition, Quaternion.Euler(Vector3.zero), null);
            listener.enabled = false; //Deactivate this audio listener just before spawning camera, which has a listener.
            magicCamera = GameObject.Instantiate(magicCameraPrefab, playerStartingPosition, Quaternion.Euler(Vector3.zero), null);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            //Turn the audio listener back on.
            listener.enabled = true;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(playerStartingPosition, 2f);
    }
}
