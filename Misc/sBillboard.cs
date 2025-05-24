using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Works with Unity QUADS!
 * Which means the Z is flipped.
 * 
 */

public class sBillboard : MonoBehaviour
{
	private GameManager gM;

    private void Start()
    {
        gM = GameManager.instance;
    }

    void Update()
	{
		if (gM.player)
        {
			Vector3 v = gM.player.transform.position - transform.position;
			v.x = v.z = 0.0f;
			transform.LookAt(gM.player.transform.position - v);
        }
	}
}
