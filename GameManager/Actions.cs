using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Actions
{
    //???
    public static Action OnEnemyDeath;
    public static Action<float, float> OnExperienceGain;
    public static Action OnPlayerDeath;

    //TERRAIN  ---------------------------

    /// <summary>
    /// isOpen
    /// </summary>
    //   public static Action<CASTLE_ID> OnCastleCreation;


    //HUD --------------------------------

    /// <summary>
    /// isOpen
    /// </summary>
    public static Action<bool> OnSpellPanelToggle;

    /// <summary>
    /// The message to display
    /// </summary>
    public static Action<string> OnHUDWarning;

    //Player -----------------------------

    /// <summary>
    /// Current Health, Max Health, isDamage
    /// </summary>
    public static Action<int, int, bool> OnHealthChange;

    /// <summary>
    /// Owner ID, Current Health, Max Health
    /// </summary>
    public static Action<OWNER_ID, int, int> OnCastleHealthChange;

    /// <summary>
    /// Current Mana, Max Mana
    /// </summary>
    public static Action<int, int> OnManaChange;

    /// <summary>
    /// OLD position
    /// </summary>
    public static Action<Vector3> OnPlayerWarp;

}