using UnityEngine;

public class sLifeTimer : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    private float lifeStart;

    private void Start()
    {
        lifeStart = Time.time;
    }

    private void Update()
    {
        if (Time.time > lifeStart + lifeTime)
        {
            Destroy(this.gameObject);
        }
    }
}
