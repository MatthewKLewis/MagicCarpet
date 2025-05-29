using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The way that preventing self damage works is by ownerName.
 * The projectile-generating character will mark the projectile
 * with the same name as itself. 
 * 
 */

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

    private void FixedUpdate()
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

            if (other.TryGetComponent(out sTerrainChunk chunkScript))
            {
                //print("Fireball hit a terrain chunk!");
                tM.AlterTerrain(transform.position);
            }

            Destroy(this.gameObject);
        }
    }
}
