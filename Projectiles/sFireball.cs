using UnityEngine;

public class sFireball : MonoBehaviour, IProjectile
{
    private sTerrainManager tM;

    private float speed = 100f;
    private float lifeTime = 3f;
    private float spawnTime;

    //IProjectile
    public bool hasHit { get; set; }
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
        //TODO - GET OWNERID OF OTHER GAME OBJECT?
        if (!hasHit)
        {
            if (other.TryGetComponent(out IKillable victimScript))
            {
                victimScript.TakeDamage(1);
            }

            //Optional terrain destruction - only within the first second of life.
            if (Time.time < spawnTime + 1f && other.GetComponent<sTerrainChunk>())
            {
                //print("Fireball hit a terrain chunk!");
                tM.ManageTerrainHit(transform.position, 1, Deformations.PockMark());
            }

            //Needed due to double hits
            hasHit = true;
            Destroy(this.gameObject);
        }
    }
}