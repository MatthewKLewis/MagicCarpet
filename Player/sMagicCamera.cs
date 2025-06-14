using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sMagicCamera : MonoBehaviour
{
    private GameManager gM;
    private Transform target;
    private Camera cam;

    [Space(10)]
    [Header("Follow Vars")]
    [SerializeField] private float followFactor;

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
        //TODO - Magic camera is instantiated by gM after player, so this doesn't fail
        target = gM.player.GetComponent<sPlayer>().cameraTransform;
    }

    private void Update()
    {
        if (target)
        {
            transform.SetPositionAndRotation(
                new Vector3(target.position.x, Mathf.Lerp(transform.position.y, target.position.y, followFactor), target.position.z), 
                target.rotation
            );
        }
    }

    private void HandlePlayerDeath()
    {
        target = null;
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
