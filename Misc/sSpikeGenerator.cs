using UnityEngine;

public class sSpikeGenerator : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print(other);
        GameManager.instance.AlterTerrain(transform.position, Deformations.HugeSpike());
        this.gameObject.SetActive(false);
    }
}
