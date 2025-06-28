using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sHUD : MonoBehaviour
{
    private GameManager gM;

    [Space(10)]
    [Header("Loading Screen")]
    [SerializeField] private Image loadingImage;
    [SerializeField] private Transform loadingScreen;
    private bool levelDone = false;
    
    [Space(10)]
    [Header("Warning")]
    [SerializeField] private Transform warningTextPanel;
    [SerializeField] private TextMeshProUGUI warningText;

    [Space(10)]
    [Header("Spell")]
    [SerializeField] private Transform spellPanel;

    [Space(10)]
    [Header("Map")]
    [SerializeField] private RectTransform playerIndicator;
    [SerializeField] private RectTransform miniMapTransform;
    [SerializeField] private RawImage miniMapImage;
    private List<GameObject> enemyIndicatorList;

    [Space(10)]
    [Header("Stats")]
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private RectTransform manaBar;
    [SerializeField] private Transform damageFrame;

    private void Awake()
    {
        gM = GameManager.instance;

        //Actions.OnCastleHealthChange += HandleCastleHealthChange;

        Actions.OnHealthChange += HandleHealthChange;
        Actions.OnManaChange += HandleManaChange;
        Actions.OnSpellPanelToggle += HandleSpellPanelToggle;
        Actions.OnEnemyDeath += HandleEnemyDeath;
        Actions.OnHUDWarning += HandleWarning;
        Actions.OnPlayerWarp += HandleWarp; 
        Actions.OnPlayerDeath += HandlePlayerDeath;
        Actions.OnLevelExit += FadeInLoadingScreen; 
    }

    private void OnDestroy()
    {
        //Actions.OnCastleHealthChange -= HandleCastleHealthChange;

        Actions.OnHealthChange -= HandleHealthChange;
        Actions.OnManaChange -= HandleManaChange;
        Actions.OnManaChange -= HandleManaChange;
        Actions.OnSpellPanelToggle -= HandleSpellPanelToggle;
        Actions.OnEnemyDeath -= HandleEnemyDeath;
        Actions.OnHUDWarning -= HandleWarning;
        Actions.OnPlayerWarp -= HandleWarp;
        Actions.OnPlayerDeath -= HandlePlayerDeath;
        Actions.OnLevelExit -= FadeInLoadingScreen;
    }

    private void Start()
    {
        spellPanel.localScale = Vector3.zero;
        damageFrame.localScale = Vector3.zero;
        warningTextPanel.localScale = Vector3.zero;
        FadeOutLoadingScreen(0);
    }

    private void HandleHealthChange(int currentHealth, int maxHealth, bool isDamage)
    {
        //change mana bar
        healthBar.localScale = new Vector2((float)currentHealth / (float)maxHealth, 1);
        
        if (isDamage)
        {
            StartCoroutine(ShowDamageFrame());
        }
    }

    private void HandleManaChange(int currentMana, int maxMana)
    {
        manaBar.localScale = new Vector2((float)currentMana / (float)maxMana, 1);
    }

    private void HandlePlayerDeath()
    {
        print("TODO - Show death screen");
    }

    private void HandleSpellPanelToggle(bool isOpen)
    {
        if (isOpen)
        {
            spellPanel.localScale = Vector3.one;
        }
        else
        {
            spellPanel.localScale = Vector3.zero;
        }
    }

    private void HandleWarp(Vector3 _pos)
    {
        HandleWarning("...");
    }

    private void HandleEnemyDeath()
    {
        return;
    }

    private IEnumerator ShowDamageFrame()
    {
        damageFrame.localScale = Vector3.one;
        float timeElapsed = 0.0f;
        while (timeElapsed < 0.25f)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        damageFrame.localScale = Vector3.zero;
    }

    private void HandleWarning(string text) { StartCoroutine(HandleWarningCoroutine(text)); }

    private IEnumerator HandleWarningCoroutine(string text)
    {
        WaitForSeconds waitOne = new WaitForSeconds(1);

        warningText.text = text;
        warningTextPanel.localScale = Vector3.one;
        yield return waitOne;
        warningText.text = "";
        warningTextPanel.localScale = Vector3.zero;
    }

    //LOADING SCREEN
    //LOADING SCREEN
    //LOADING SCREEN

    private void FadeInLoadingScreen(int _l) { StartCoroutine(FadeInLoadingScreenCoroutine()); }
    private IEnumerator FadeInLoadingScreenCoroutine()
    {
        //Run on end - screen alpha goes from 100 to 0.
        if (levelDone) yield break; //Don't do this coroutine twice.
        for (int i = 0; i <= 100; i++)
        {
            loadingImage.color = new Color(0, 0, 0, i / 100f);
            yield return null;
        }
        levelDone = true;
    }

    private void FadeOutLoadingScreen(int _l) { StartCoroutine(FadeOutLoadingScreenCoroutine()); }
    private IEnumerator FadeOutLoadingScreenCoroutine()
    {
        //Run on start - screen alpha goes from 100 to 0.
        for (int i = 100; i >= 0; i--)
        {
            loadingImage.color = new Color(0, 0, 0, i / 100f);
            yield return null;
        }
    }
}
