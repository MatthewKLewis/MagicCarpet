using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable
{
    int currentHealth { get; set; }
    int maxHealth { get; set; }
    bool isDead { get; set; }
    public bool TakeDamage(int damage);
}


/*
 * The way that preventing self damage works is by ownerName.
 * The projectile-generating character will mark the projectile
 * with the same name as itself. 
 * MKL
 */
public interface IProjectile
{
    string ownerName { get; set; }
}