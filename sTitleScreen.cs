using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTitleScreen : MonoBehaviour
{
    private GameManager gM;

    void Start()
    {
        gM = GameManager.instance;
    }

    public void HandlePlayButton()
    {
        gM.LoadLevel(1);
    }

    public void HandleQuitButton()
    {
        gM.QuitGame();
    }
}
