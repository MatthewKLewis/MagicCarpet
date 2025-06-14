using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTitleScreen : MonoBehaviour
{
    private SessionManager sM;

    void Start()
    {
        sM = SessionManager.instance;
    }

    public void HandlePlayButton()
    {
        sM.LoadLevel(1);
    }

    public void HandleQuitButton()
    {
        sM.QuitGame();
    }
}
