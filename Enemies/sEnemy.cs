using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sEnemy : MonoBehaviour, IKillable
{
    private GameManager gM;

    //Unity
    private CharacterController cC;
    [SerializeField] private LayerMask terrainMask;

    //State
    private float yComponentOfMovement;

    //Health
    public int currentHealth { get; set; } = 10;
    public int maxHealth { get; set; } = 10;
    public bool isDead { get; set; } = false;

    //AI Info
    private float distanceToPlayer;

    //Multipliers
    private float gravity = -1f;

    //Prefabs
    [SerializeField] private GameObject fireballPrefab;
    private float timeLastShot = -0.5f;
    private float shotCooldown = 0.5f;


    void Start()
    {
        gM = GameManager.instance;
        cC = GetComponent<CharacterController>();
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
            Instantiate(fireballPrefab, transform.position + new Vector3(0, 1, 1), transform.rotation, null);
        }
    }

    public bool TakeDamage(int damage)
    {
        print("Owwie!!");
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
        Instantiate(gM.manaOrb, transform.position + Vector3.up, transform.rotation, null);
    }
}
