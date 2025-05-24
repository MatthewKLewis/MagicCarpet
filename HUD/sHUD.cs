using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sHUD : MonoBehaviour
{
    private GameManager gM;

    [SerializeField] private Transform spellPanel;
    [SerializeField] private RectTransform playerIndicator;

    private void Awake()
    {
        Actions.OnHealthChange += HandleHealthChange;
        Actions.OnManaChange += HandleManaChange;
        Actions.OnSpellPanelToggle += HandleSpellPanelToggle;
    }

    private void OnDestroy()
    {
        Actions.OnHealthChange -= HandleHealthChange;
        Actions.OnManaChange -= HandleManaChange;
        Actions.OnManaChange -= HandleManaChange;
        Actions.OnSpellPanelToggle += HandleSpellPanelToggle;
    }

    private void Start()
    {
        gM = GameManager.instance;
        spellPanel.localScale = Vector3.zero;
    }

    private void Update()
    {
        //Move the player indicator
        if (gM.player)
        {
            playerIndicator.localPosition =
                (new Vector2(gM.player.transform.position.x, gM.player.transform.position.z) - new Vector2(1488, 1488)) / (1488f / 120f);
        }
    }

    private void HandleHealthChange(float old, float nu)
    {

    }

    private void HandleManaChange(float old, float nu)
    {

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
}
