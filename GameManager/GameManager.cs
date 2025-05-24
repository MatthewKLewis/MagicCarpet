using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
        //Application.targetFrameRate = 90;
    }
}
