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
    public OWNER_ID ownerID = OWNER_ID.NPC_1;

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
    private int currentMana = 3;
    private int maxMana = 20;
    private float timeLastRegen = -1f;
    private float regenCooldown = 1f;

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

        //Regen
        RegenHealthAndMana();

        //FX
        wakeAndDust.GenerateWakeOrDust(cC.velocity.magnitude, groundHit.point.y, distanceToGround);
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
        if (distanceToPlayer > 10f)
        {
            movement = new Vector3(normalVectorToPlayer.x, 0, normalVectorToPlayer.z) / 2f; //MAGIC NUMBER
        }

        //Recomposing
        movement.y = yComponentOfMovement;

        //Send it!
        cC.Move(movement);

        //COMBAT
        spellOrigin.LookAt(gM.player.transform);
        if (distanceToPlayer < 10f)
        {
            Shoot();
        }
    }

    /*
     * 
     * Combat
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
                .GetComponent<IProjectile>().ownerID = ownerID;
        }
    }

    public void RegenHealthAndMana()
    {
        if (Time.time > timeLastRegen + regenCooldown)
        {
            timeLastRegen = Time.time;
            currentHealth += 1;
            currentMana += 1;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            currentMana = Mathf.Clamp(currentMana, 0, maxMana);
            Actions.OnHealthChange.Invoke(currentHealth, maxHealth, false);
            Actions.OnManaChange.Invoke(currentMana, maxMana);
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
        // THIS ONLY MARKS A NEW AI STATE FOR THE ENEMY, THE BEHAVIORS RELEVANT
        // TO THE AI ARE IN UPDATE
        // TODO - Change AI state based on current factors like Life, Mana, Castle Damage

        while (gM.player)
        {
            yield return new WaitForSeconds(3);

            float healthPercent = currentHealth / maxHealth;
            if (healthPercent < 0.5f)
            {
                //Actions.OnHUDWarning(this.gameObject.name + " is now retreating.");
                aiState = AIState.RETREATING;
            }
            else if (distanceToPlayer < 50f)
            {
                //Actions.OnHUDWarning(this.gameObject.name + " is now attacking.");
                aiState = AIState.ATTACKING;
            }
            else if (Vector3.Distance(transform.position, homeBase) < 10f)
            {
                //Actions.OnHUDWarning(this.gameObject.name + " is now roaming.");

                //TODO - Pick a roaming spot

                aiState = AIState.ROAMING;
            }
            else
            {
                //Actions.OnHUDWarning(this.gameObject.name + " didn't change his mind and is still " + aiState.ToString());
            }
        }
    }
}
