using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Actions
{
    //many to many
    public static Action<int> OnEnemyDeath;
    public static Action<float, float> OnExperienceGain;
    public static Action OnPlayerDeath;

    //conversations
    public static Action OnConversationStart;
    public static Action OnConversationEnd;

    //from HUD to GAME
    public static Action<bool> OnSpellPanelToggle;

    //Journal
    public static Action<int> OnPlotEvent;

    //Player
    public static Action<float, float> OnHealthChange;
    public static Action<float, float> OnManaChange;
}