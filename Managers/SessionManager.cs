using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : MonoBehaviour
{
    public static SessionManager instance;

    private void Awake()
    {
        //SINGLETON, one per GAME SESSION
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        //Actions
        Actions.OnLevelExit += HandleLevelExit;
    }

    private void OnDestroy()
    {
        Actions.OnLevelExit -= HandleLevelExit;
    }

    void Start()
    {
        //print("Hello Session Manager!");
    }

    public void LoadLevel(int level) { StartCoroutine(GoToSceneCoroutine(level)); }
    private IEnumerator GoToSceneCoroutine(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        //Show mouse, spawn nothing
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        while (!operation.isDone)
        {
            print(operation.progress);
            //float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }
    }

    private void HandleLevelExit(int levelIndex)
    {
        LoadLevel(levelIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
