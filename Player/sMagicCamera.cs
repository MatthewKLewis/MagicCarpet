using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sMagicCamera : MonoBehaviour
{
    private GameManager gM;
    private Transform target;
    private Camera cam;

    private void Awake()
    {
        gM = GameManager.instance;
        cam = GetComponent<Camera>();

        Actions.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDestroy()
    {
        Actions.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void Start()
    {
        target = gM.player.GetComponent<sPlayer>().cameraTransform;
    }

    private void Update()
    {
        if (target)
        {
            transform.rotation = target.rotation;
            transform.position = target.position;
        }
    }

    private void HandlePlayerDeath()
    {
        return;
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
