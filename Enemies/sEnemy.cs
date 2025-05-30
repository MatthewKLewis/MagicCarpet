using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIState
{
    ROAMING    = 0,
    ATTACKING  = 1,
    RETREATING = 2,
    COLLECTING = 3,
}

public class sEnemy : MonoBehaviour, IKillable
{
    private GameManager gM;

    //Unity
    private CharacterController cC;
    [SerializeField] private AudioSource enemyAudioSource;
    [SerializeField] private LayerMask terrainMask;
    [SerializeField] private Transform spellOrigin;

    //State
    private float yComponentOfMovement;

    //Health
    public int currentHealth { get; set; } = 10;
    public int maxHealth { get; set; } = 10;
    public bool isDead { get; set; } = false;

    //AI Info
    private float distanceToPlayer;
    private Vector3 homeBase;

    //Prefabs
    private float timeLastShot = -0.5f;
    private float shotCooldown = 0.5f;

    [Space(10)]
    [Header("Wake and Dust")]
    [SerializeField] private sWakeAndDust wakeAndDust;

    void Start()
    {
        gM = GameManager.instance;
        cC = GetComponent<CharacterController>();

        homeBase = transform.position;

        StartCoroutine(ChangeAIState());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }


    private void Update()
    {
        Vector3 normalVectorToPlayer = Vector3.zero;
        float healthPercent = currentHealth / maxHealth;

        //Gather information
        if (gM.player)
        {
            transform.LookAt(gM.player.transform);
            spellOrigin.LookAt(gM.player.transform);
            normalVectorToPlayer = (gM.player.transform.position - transform.position).normalized;
            distanceToPlayer = Vector3.Distance(transform.position, gM.player.transform.position);
        }

        //Probe ground distance
        RaycastHit groundHit;
        float distanceToGround = 0;
        if (Physics.Raycast(transform.position, Vector3.down, out groundHit, Mathf.Infinity, terrainMask))
        {
            distanceToGround = groundHit.distance;
        }

        //Y movement
        yComponentOfMovement = -distanceToGround * Time.deltaTime;

        //Horizontal movement
        Vector3 movement = Vector3.zero;
        if (distanceToPlayer > 10f)
        {
            movement = new Vector3(normalVectorToPlayer.x, 0, normalVectorToPlayer.z) / 2f; //MAGIC NUMBER
        }

        //Recomposing
        movement.y = yComponentOfMovement;

        //Send it!
        cC.Move(movement);

        if (distanceToPlayer < 10f)
        {
            Shoot();
        }

        //Wake and Dust
        wakeAndDust.GenerateWakeOrDust(cC.velocity.magnitude, groundHit.point.y, distanceToGround);
    }

    private void FixedUpdate()
    {
        //Update used to just have the Shoot logic, now everything is in Update and FixedUpdate is empty.
    }

    private void Shoot()
    {
        if (Time.time > timeLastShot + shotCooldown)
        {
            timeLastShot = Time.time;
            enemyAudioSource.PlayOneShot(gM.fireBallClip);

            //Mark projectile with ownerName!
            float randomDeg = Mathf.Deg2Rad * Random.Range(-15, 15);
            Instantiate(gM.fireBallPrefab, spellOrigin.position, Quaternion.LookRotation(spellOrigin.forward + new Vector3(randomDeg, 0, randomDeg)), null)
                .GetComponent<IProjectile>().ownerName = this.gameObject.name; ;
        }
    }

    public bool TakeDamage(int damage)
    {
        //print("Owwie!!");
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if (currentHealth == 0)
        {
            Die();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Die()
    {
        SpawnManaReward();
        Destroy(this.gameObject);
    }

    private void SpawnManaReward()
    {
        Instantiate(gM.manaOrbPrefab, transform.position + Vector3.up, transform.rotation, null);
    }

    private IEnumerator ChangeAIState()
    {
        while (gM.player)
        {
            yield return new WaitForSeconds(3);
            print("Making AI decision to...");
            print("KILL THE PLAYER");
            //TODO - Change AI state based on current factors like Life, Mana, Castle Damage
        }
    }
}
