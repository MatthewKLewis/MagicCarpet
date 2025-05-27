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

public interface IProjectile
{
    string ownerName { get; set; }
}