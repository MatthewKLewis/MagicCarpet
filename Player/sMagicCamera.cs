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
        target = gM.player.GetComponent<sPlayer>().cameraTransform;
    }

    private void Update()
    {
        if (target)
        {
            transform.rotation = target.rotation;
            //transform.position = target.position;
            transform.position = new Vector3(
                target.position.x, 
                Mathf.Lerp(transform.position.y, target.position.y, followFactor), 
                target.position.z)
            ;
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
