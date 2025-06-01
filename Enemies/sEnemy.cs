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

    //AI
    private AIState aiState = AIState.ROAMING;

    //State
    private float distanceToGround;
    private float yComponentOfMovement;
    private RaycastHit groundHit;

    //Health
    public int currentHealth { get; set; } = 10;
    public int maxHealth { get; set; } = 10;
    public bool isDead { get; set; } = false;

    //AI Info
    private float distanceToPlayer;
    private Vector3 homeBase;
    private Vector3 roamTarget;

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

    /*
     * 
     * Update and _U Functions
     * 
     */


    private void Update()
    {
        if (gM.player)
        {
            // Most important piece of AI info is collected on frame.
            distanceToPlayer = Vector3.Distance(transform.position, gM.player.transform.position);

            //Probe ground distance
            if (Physics.Raycast(transform.position, Vector3.down, out groundHit, Mathf.Infinity, terrainMask))
            {
                distanceToGround = groundHit.distance;
            }

            switch (aiState)
            {
                case AIState.ROAMING:
                    Roam_U();
                    break;
                case AIState.ATTACKING:
                    Attack_U();
                    break;
                case AIState.RETREATING:
                    Retreat_U();
                    break;
                case AIState.COLLECTING:
                    //Nothing?
                    break;
                default:
                    Roam_U();
                    break;
            }        
        }
        else
        {
            Roam_U(); //Just mill around if the player is dead.
        }

        wakeAndDust.GenerateWakeOrDust(cC.velocity.magnitude, groundHit.point.y, distanceToGround);
    }

    private void Roam_U()
    {
        //TODO - move towards RoamTarget
    }

    private void Retreat_U()
    {
        transform.LookAt(homeBase);
        spellOrigin.LookAt(homeBase);
        Vector3 normalVectorToHomeBase = (homeBase - transform.position).normalized;

        //Y movement
        yComponentOfMovement = -distanceToGround * Time.deltaTime;

        //Horizontal movement
        Vector3 movement = new Vector3(normalVectorToHomeBase.x, 0, normalVectorToHomeBase.z) / 2f; //MAGIC NUMBER
        movement.y = yComponentOfMovement;        

        //Send it!
        cC.Move(movement);

        //Wake and Dust
    }

    private void Attack_U()
    {
        transform.LookAt(gM.player.transform);
        spellOrigin.LookAt(gM.player.transform);
        Vector3 normalVectorToPlayer = (gM.player.transform.position - transform.position).normalized;        

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

        //COMBAT
        if (distanceToPlayer < 10f)
        {
            Shoot();
        }
    }

    /*
     * 
     * Beginneth the section for combat
     * 
     */

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

    /*
     * 
     * Coroutines
     * 
     */

    private IEnumerator ChangeAIState()
    {
        // THIS ONLY MARKS A NEW AI STATE FOR THE ENEMY, THE BEHAVIORS RELEVANT
        // TO THE AI ARE IN UPDATE
        // TODO - Change AI state based on current factors like Life, Mana, Castle Damage

        while (gM.player)
        {
            yield return new WaitForSeconds(3);

            float healthPercent = currentHealth / maxHealth;
            if (healthPercent < 0.5f)
            {
                print(this.gameObject.name + " is now retreating.");
                aiState = AIState.RETREATING;
            }
            else if (distanceToPlayer < 50f)
            {
                print(this.gameObject.name + " is now attacking.");
                aiState = AIState.ATTACKING;
            }
            else if (Vector3.Distance(transform.position, homeBase) < 2f)
            {
                print(this.gameObject.name + " is now roaming.");

                //TODO - Pick a roaming spot if you're gonna roam

                aiState = AIState.ROAMING;
            }
        }
    }
}
