using UnityEngine;

public class sCastleSeed : MonoBehaviour, IProjectile
{
    //TODO - POOL THESE PROJECTILES!

    private GameManager gM;

    private float speed = 100f;
    private float lifeTime = 3f;
    private float spawnTime;

    //IProjectile
    public bool hasHit { get; set; }
    public int damage { get; set; }
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
            if (other.GetComponent<sTerrainChunk>())
            {
                int casterCastleLevel = gM.castleInfo[(int)ownerID].level;
                switch (casterCastleLevel)
                {
                    case 0: gM.AlterTerrain(transform.position, Deformations.CastleOrigin(ownerID)); break;
                    case 1: gM.AlterTerrain(transform.position, Deformations.CastleUpgrade_2(ownerID)); break;
                    default: print("Cant find player's castle level / upgrade above 2!"); break;
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
