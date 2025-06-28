using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OWNER_ID
{
    PLAYER,
    NPC_1,
}

public interface IKillable
{
    int currentHealth { get; set; }
    int maxHealth { get; set; }
    bool isDead { get; set; }
    public bool TakeDamage(int damage);
}

public interface IProjectileSpawner
{
    public OWNER_ID ownerID { get; set; }
}


/*
 * The way that preventing self damage works is by ownerName.
 * The projectile-generating character will mark the projectile
 * with the same name as itself. 
 * MKL
 */
public interface IProjectile
{
    OWNER_ID ownerID { get; set; }
    public int damage { get; set; }
    bool hasHit { get; set; }
}