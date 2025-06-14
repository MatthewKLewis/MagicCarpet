using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Works with Unity QUADS!
 * Which means the Z is flipped.
 * 
 */

public class sLookAt : MonoBehaviour
{
    private GameManager gM;
    private Transform target = null;

    private void Awake()
    {
        gM = GameManager.instance;
    }

    void Update()
    {
        if (target)
        {
            transform.LookAt(target);
        }
        else
        {
            if (gM.player)
            {
                target = gM.player.GetComponent<sPlayer>().cameraTransform;
            }
        }
    }
}
