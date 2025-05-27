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
    private float speed = 100f;
    private float lifeTime = 3f;
    private float spawnTime;

    public string ownerName { get; set; }

    private void Start()
    {
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
            if (other.TryGetComponent(out IKillable scriptOfKillable))
            {
                scriptOfKillable.TakeDamage(1);
            }

            Destroy(this.gameObject);
        }
    }
}
