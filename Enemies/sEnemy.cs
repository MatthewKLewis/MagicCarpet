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

    //[SerializeField] private LayerMask terrainMask; //Floaty enemies also?

    //State
    private float yComponentOfMovement;

    //Health
    public int currentHealth { get; set; } = 10;
    public int maxHealth { get; set; } = 10;
    public bool isDead { get; set; } = false;

    //AI Info
    private float distanceToPlayer;
    private Vector3 homeBase;

    //Multipliers
    private float gravity = -1f;

    //Prefabs
    private float timeLastShot = -0.5f;
    private float shotCooldown = 0.5f;

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
        if (distanceToPlayer < 10f)
        {
            Shoot();
        }
    }
    private void FixedUpdate()
    {
        Vector3 normalVectorToPlayer = Vector3.zero;
        float healthPercent = currentHealth / maxHealth;

        //Gather information
        if (gM.player)
        {
            transform.LookAt(gM.player.transform);

            normalVectorToPlayer = (gM.player.transform.position - transform.position).normalized;
            distanceToPlayer = Vector3.Distance(transform.position, gM.player.transform.position);
        }

        //Gravity
        yComponentOfMovement += (gravity * Time.deltaTime);
        if (cC.isGrounded) { yComponentOfMovement = 0.0f; }

        //Horizontal movement
        Vector3 movement = Vector3.zero;
        if (distanceToPlayer > 10f)
        {
            movement = new Vector3(normalVectorToPlayer.x, 0, normalVectorToPlayer.z) / 2f; //MAGIC NUMBER
        }

        //Recomposing
        movement.y = yComponentOfMovement;

        //Print!
        cC.Move(movement);
    }

    private void Shoot()
    {
        if (Time.time > timeLastShot + shotCooldown)
        {
            timeLastShot = Time.time;
            enemyAudioSource.PlayOneShot(gM.fireBallClip);

            //Mark projectile with ownerName!
            //TODO Random scatter needed!
            Instantiate(gM.fireBallPrefab, transform.position + new Vector3(0, 1, 1), transform.rotation, null)
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
