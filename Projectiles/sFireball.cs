using UnityEngine;

public class sFireball : MonoBehaviour, IProjectile
{
    private sTerrainManager tM;

    private float speed = 100f;
    private float lifeTime = 3f;
    private float spawnTime;


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
            if (other.TryGetComponent(out IKillable victimScript))
            {
                victimScript.TakeDamage(1);
            }

            //Optional terrain destruction - only within the first second of life.
            if (Time.time < spawnTime + 1f && other.GetComponent<sTerrainChunk>())
            {
                //print("Fireball hit a terrain chunk!");
                tM.AlterTerrain(transform.position, Deformations.returnPockMark());
            }

            Destroy(this.gameObject);
        }
    }
}