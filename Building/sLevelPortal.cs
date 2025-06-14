using System.Collections;
using UnityEngine;

public class sLevelPortal : MonoBehaviour
{
    [SerializeField] private int levelToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Actions.OnLevelExit.Invoke(levelToLoad);
        }
    }
}
