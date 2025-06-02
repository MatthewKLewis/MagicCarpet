using UnityEngine;

public class sMana : MonoBehaviour
{
    [SerializeField] private LayerMask terrainMask;

    // Update is called once per frame
    void Update()
    {
        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, Mathf.Infinity, terrainMask))
        {
            transform.position = Vector3.Lerp(transform.position, groundHit.point + Vector3.up, 0.01f);
        }
    }
}
