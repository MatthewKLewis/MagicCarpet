using UnityEngine;

public class sFireball : MonoBehaviour, IProjectile
{
    //TODO - POOL THESE PROJECTILES!

    private GameManager gM;

    private float speed = 100f;
    private float lifeTime = 3f;
    private float spawnTime;

    //IProjectile
    public bool hasHit { get; set; }
    public int damage { get; set; } = 1;
    public OWNER_ID ownerID { get; set; }

    private void Start()
    {
        gM = GameManager.instance;
        spawnTime = Time.time;
    }

    private void Update()
    {
        //Move at 100mps
        transform.position += transform.forward * speed * Time.deltaTime;

        //TODO - Raycast forward, check for hit in next frame, run the trigger enter
        //logic. USE HAS HIT!

        //Die after 5 seconds
        if (Time.time > spawnTime + lifeTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasHit)
        {
            if (other.TryGetComponent(out IKillable victimScript))
            {
                if (other.TryGetComponent(out IProjectileSpawner iProjectileSpawner))
                {
                    if (iProjectileSpawner.ownerID == ownerID)
                    {
                        return; //DO NOT INTERACT
                    }
                }
                victimScript.TakeDamage(damage);
            }

            //Optional terrain destruction - only within the first second of life.
            if (Time.time < spawnTime + 1f && other.GetComponent<sTerrainChunk>())
            {
                //print("Fireball hit a terrain chunk!");
                gM.AlterTerrain(transform.position, Deformations.PockMark(), damage);
            }

            //Needed due to double hits
            hasHit = true;
            Instantiate(GameManager.instance.smallExplosionEffectPrefab, transform.position, transform.rotation, null);
            Destroy(this.gameObject);
        }
    }
}