using UnityEngine;

public class sMiniMapCamera : MonoBehaviour
{
    private GameManager gM;
    private Transform playerTransform;

    void Start()
    {
        gM = GameManager.instance;
        playerTransform = gM.player.transform;
    }

    void Update()
    {
        if (playerTransform)
        {
            transform.SetPositionAndRotation(
                new Vector3(playerTransform.position.x, 300, playerTransform.position.z),
                Quaternion.Euler(new Vector3(90f, playerTransform.rotation.eulerAngles.y, 0f))
            );
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
