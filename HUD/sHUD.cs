using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sHUD : MonoBehaviour
{
    private GameManager gM;
    private sTerrainManager tM;

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

    private void Awake()
    {
        Actions.OnHealthChange += HandleHealthChange;
        Actions.OnManaChange += HandleManaChange;
        Actions.OnSpellPanelToggle += HandleSpellPanelToggle;
        Actions.OnEnemyDeath += HandleEnemyDeath;
    }

    private void OnDestroy()
    {
        Actions.OnHealthChange -= HandleHealthChange;
        Actions.OnManaChange -= HandleManaChange;
        Actions.OnManaChange -= HandleManaChange;
        Actions.OnSpellPanelToggle -= HandleSpellPanelToggle;
        Actions.OnEnemyDeath -= HandleEnemyDeath;
    }

    private void Start()
    {
        gM = GameManager.instance;
        tM = sTerrainManager.instance;
        spellPanel.localScale = Vector3.zero;
    }

    private void Update()
    {
        //Move the player indicator
        if (gM.player)
        {
            Vector3 playPos = gM.player.transform.position;
            Vector3 playRot = gM.player.transform.rotation.eulerAngles;

            int fullWidth = tM.chunks.GetLength(0) * (tM.CHUNK_WIDTH - 1) * tM.TILE_WIDTH;
            miniMapImage.uvRect = new Rect(
                (playPos.x - (fullWidth/2)) / fullWidth, 
                (playPos.z - (fullWidth/2)) / fullWidth, 
                3, 
                3
            );
            miniMapTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, playRot.y));
        }
    }

    private void HandleHealthChange(int currentHealth, int maxHealth)
    {
        //change health bar
        healthBar.localScale = new Vector2((float)currentHealth / (float)maxHealth, 1) ;
    }

    private void HandleManaChange(int currentMana, int maxMana)
    {
        //change mana bar
        healthBar.localScale = new Vector2((float)currentMana / (float)maxMana, 1);
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

    private void HandleEnemyDeath()
    {
        return;
    }
}
