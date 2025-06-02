using UnityEngine;

public class sCastleSeed : MonoBehaviour, IProjectile
{
    private sTerrainManager tM;

    private float speed = 100f;
    private float lifeTime = 3f;
    private float spawnTime;

    //IProjectile
    public bool hasHit { get; set; }
    public int damage { get; set; }
    public OWNER_ID ownerID { get; set; }

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
        if (!hasHit)
        {
            if (other.GetComponent<sTerrainChunk>())
            {
                if (tM.castleInfo[(int)ownerID].level == 0)
                {
                    tM.CreateCastle(transform.position, ownerID);
                }
                else
                {
                    tM.UpgradeCastle(transform.position, ownerID);
                }
            }

            // If the other component is a projectile spawner with the same ID as
            // the owner ID of this projectile - abort.
            if (other.TryGetComponent(out IProjectileSpawner iProjectileSpawner))
            {
                if (iProjectileSpawner.ownerID == ownerID)
                {
                    //DO NOT INTERACT
                    return;
                }
            }
            //Needed due to double hits
            hasHit = true;
            Destroy(this.gameObject);            
        }
    }
}
