using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sHUD : MonoBehaviour
{
    private GameManager gM;
    private GameManager tM;

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

    [Space(10)]
    [Header("Stats")]
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private RectTransform manaBar;
    [SerializeField] private Transform damageFrame;


    private void Awake()
    {
        Actions.OnHealthChange += HandleHealthChange;
        Actions.OnManaChange += HandleManaChange;

        Actions.OnSpellPanelToggle += HandleSpellPanelToggle;

        Actions.OnEnemyDeath += HandleEnemyDeath;

        Actions.OnHUDWarning += HandleWarning;

        Actions.OnPlayerWarp += HandleWarp;
    }

    private void OnDestroy()
    {
        Actions.OnHealthChange -= HandleHealthChange;
        Actions.OnManaChange -= HandleManaChange;

        Actions.OnManaChange -= HandleManaChange;
        Actions.OnSpellPanelToggle -= HandleSpellPanelToggle;

        Actions.OnEnemyDeath -= HandleEnemyDeath;

        Actions.OnHUDWarning -= HandleWarning;

        Actions.OnPlayerWarp -= HandleWarp;
    }

    private void Start()
    {
        gM = GameManager.instance;
        tM = GameManager.instance;

        spellPanel.localScale = Vector3.zero;
        damageFrame.localScale = Vector3.zero;
        warningTextPanel.localScale = Vector3.zero;
    }

    private void Update()
    {
        //Move the player indicator
        if (gM.player)
        {
            Vector3 playPos = gM.player.transform.position;
            Vector3 playRot = gM.player.transform.rotation.eulerAngles;

            int fullWidth = gM.chunks.GetLength(0) * (gM.CHUNK_WIDTH - 1) * gM.TILE_WIDTH;
            miniMapImage.uvRect = new Rect(
                (playPos.x - (fullWidth/2)) / fullWidth, 
                (playPos.z - (fullWidth/2)) / fullWidth, 
                3, 
                3
            );
            miniMapTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, playRot.y));
        }
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
        warningText.text = text;
        warningTextPanel.localScale = Vector3.one;
        yield return new WaitForSeconds(1f);
        warningText.text = "";
        warningTextPanel.localScale = Vector3.zero;
    }
}
