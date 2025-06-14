using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sCritter : MonoBehaviour, IKillable
{
    private GameManager gM;
    
    //Health
    public int currentHealth { get; set; } = 3;
    public int maxHealth { get; set; } = 3;
    public bool isDead { get; set; } = false;

    private void Awake()
    {
        gM = GameManager.instance;        
    }

    void Start()
    {
        //
    }

    void Update()
    {
        //
    }

    public bool TakeDamage(int damage)
    {
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
        gM.SpawnManaFromPool(transform.position);
        Destroy(this.gameObject);
    }
}
