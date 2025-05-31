using UnityEngine;

public class sSpikifier : MonoBehaviour, IProjectile
{
    private sTerrainManager tM;

    private float speed = 100f;
    private float lifeTime = 3f;
    private float spawnTime;

    private int[,] spikeTemplate = new int[3, 3] {
        { 3, 3, 3, },
        { 3, 3, 3, },
        { 3, 3, 3, },
    };


    public string ownerName { get; set; }

    private void Start()
    {
        tM = sTerrainManager.instance;
        spawnTime = Time.time;
    }

    private void Update()
    {
        //Move at 100mps
        transform.position += transform.forward * speed * Time.deltaTime;

        //Die after 5 seconds
        if (Time.time > spawnTime + lifeTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != ownerName)
        {
            if (other.TryGetComponent(out sTerrainChunk chunkScript))
            {
                //print("Fireball hit a terrain chunk!");
                tM.AlterTerrain(transform.position);
            }

            Destroy(this.gameObject);
        }
    }
}
