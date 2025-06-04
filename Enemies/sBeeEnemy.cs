using System.Collections;
using UnityEngine;

public class sBeeEnemy : MonoBehaviour, IKillable
{
    private GameManager gM;

    //Unity
    private CharacterController cC;
    [SerializeField] private AudioSource enemyAudioSource;
    [SerializeField] private LayerMask terrainMask;

    //[SerializeField] private LayerMask terrainMask; //Floaty enemies also? 

    //AI
    private AIState aiState = AIState.ROAMING;

    //State
    private float distanceToGround;
    private float yComponentOfMovement;
    private RaycastHit groundHit;

    //Health
    public int currentHealth { get; set; } = 3;
    public int maxHealth { get; set; } = 3;
    public bool isDead { get; set; } = false;

    //AI Info
    private float distanceToPlayer;
    private Vector3 homeBase;

    //Attacks
    private float timeLastShot = -0.5f;
    private float shotCooldown = 0.5f;

    //Movement
    [Space(4)]
    [Range(0.1f, 1.0f)]
    [SerializeField] private float speedPenalty = 0.1f;

    private void Awake()
    {
        gM = GameManager.instance;
        cC = GetComponent<CharacterController>();        
    }

    void Start()
    {
        homeBase = transform.position;
        StartCoroutine(ChangeAIState());
        //print("Bee ready!");
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
    }

    private void Roam_U()
    {
        YawLookAt(homeBase);
        Vector3 normalVectorToRoamTarget = (homeBase - transform.position).normalized;

        //Y movement
        yComponentOfMovement = -distanceToGround * Time.deltaTime;

        //Horizontal movement
        Vector3 movement = new Vector3(normalVectorToRoamTarget.x, 0, normalVectorToRoamTarget.z) / 2f; //MAGIC NUMBER
        movement.y = yComponentOfMovement;

        //Send it!
        cC.Move(movement);

        //Wake and Dust
    }

    private void Retreat_U()
    {
        YawLookAt(homeBase);
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
        YawLookAt(gM.player.transform.position);
        Vector3 normalVectorToPlayer = (gM.player.transform.position - transform.position).normalized;

        //Y movement
        yComponentOfMovement = -distanceToGround * Time.deltaTime;

        //Horizontal movement
        //TODO - Flocking / Simpler separation pushes
        Vector3 movement = Vector3.zero;
        if (distanceToPlayer > 2f)
        {
            movement = new Vector3(normalVectorToPlayer.x, 0, normalVectorToPlayer.z) * speedPenalty; //MAGIC NUMBER
        }

        //Recomposing
        movement.y = yComponentOfMovement;

        //Send it!
        cC.Move(movement);

        //COMBAT
        if (distanceToPlayer < 3f)
        {
            Sting();
        }
    }

    /*
     * 
     * Combat
     * 
     */

    private void Sting()
    {
        if (Time.time > timeLastShot + shotCooldown)
        {
            timeLastShot = Time.time;
            gM.player.GetComponent<IKillable>().TakeDamage(1);
        }
    }

    public bool TakeDamage(int damage)
    {
        print("Ow! " + this.gameObject.name + " now has " + currentHealth + " / " + maxHealth);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        speedPenalty = 0.05f;

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
        //Instantiate(gM.manaOrbPrefab, transform.position + Vector3.up, transform.rotation, null);
    }


    /*
     * 
     * Utility
     * 
     */

    private void YawLookAt(Vector3 target)
    {
        Vector3 v = target - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(target - v);
    }


    /*
     * 
     * Coroutines
     * 
     */
    private IEnumerator ChangeAIState()
    {
        while (gM.player) {
            //print(gameObject.name + " change AI state");

            yield return new WaitForSeconds(3);

            //Just changing the speed penalties of the
            //Bees might help to break up their grouping
            speedPenalty = Random.Range(0.1f, 0.5f);

            if (distanceToPlayer < 100f)
            {
                aiState = AIState.ATTACKING;
            }
            else
            {
                aiState = AIState.RETREATING;
            }
        }
    }
}
