using UnityEngine;

public class sCastleSeed : MonoBehaviour, IProjectile
{
    private sTerrainManager tM;

    private float speed = 100f;
    private float lifeTime = 3f;
    private float spawnTime;

    //IProjectile
    public bool hasHit { get; set; }
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
        if (!hasHit && other.gameObject.name != ownerName)
        {
            if (other.GetComponent<sTerrainChunk>())
            {
                //print(Time.time + " Castler hit.");
                tM.ManageCastleCreation(transform.position, Deformations.ReturnCastleOrigin());
            }
            //Needed due to double hits
            hasHit = true;
            Destroy(this.gameObject);
        }
    }
}
