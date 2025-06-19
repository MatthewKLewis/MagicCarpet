using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class sLevelEditorHUD : MonoBehaviour
{
    [SerializeField] private Transform editorPanel;

    [SerializeField] private Transform uvIndexButtonParent;
    [SerializeField] private GameObject uvIndexButtonPrefab;

    [SerializeField] private TextMeshProUGUI brushText;

    [SerializeField] private Slider heightSlider;

    private void Awake()
    {
        Actions.OnSpellPanelToggle += HandlePanelToggle;
        Actions.OnHUDWarning += HandleHUDWarning;
    }

    private void OnDestroy()
    {
        Actions.OnSpellPanelToggle -= HandlePanelToggle;
        Actions.OnHUDWarning -= HandleHUDWarning;

    }

    void Start()
    {
        for (int i = 0; i < 64; i++)
        {
            GameObject gO = Instantiate(uvIndexButtonPrefab, uvIndexButtonParent);
            sUVIndexButton script = gO.GetComponent<sUVIndexButton>();
            script.SetUVIndex(i);
        }

        heightSlider.onValueChanged.AddListener(delegate{HandleHeightSliderChange();});


        editorPanel.localScale = Vector3.zero;
    }

    public void HandleHUDWarning(string message)
    {
        print(message);
    }

    public void HandleSaveButtonClick()
    {
        print("Saving a Level... ");
        Actions.OnLevelEditorImageSave.Invoke();
    }

    public void HandleDrawTerrainButtonClick()
    {
        brushText.text = "TERRAIN";
        Actions.OnLevelEditorDrawTerrainModeEntered.Invoke();
    }

    public void HandleDrawStructureButtonClick()
    {
        brushText.text = "STRUCTURE";
        Actions.OnLevelEditorDrawStructureModeEntered.Invoke();
    }

    public void HandleHeightSliderChange()
    {
        print(heightSlider.value);
        Actions.OnLevelEditorHeightChange.Invoke((int)heightSlider.value);
    }

    public void HandlePanelToggle(bool show)
    {
        if (show)
        {
            editorPanel.localScale = Vector3.one;
        }
        else
        {
            editorPanel.localScale = Vector3.zero;
        }
    }
}
