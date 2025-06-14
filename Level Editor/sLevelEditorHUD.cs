using UnityEngine;

public class sLevelEditorHUD : MonoBehaviour
{
    [SerializeField] private Transform topPanel;
    [SerializeField] private Transform botPanel;

    private void Awake()
    {
        Actions.OnSpellPanelToggle += HandlePanelToggle;
    }

    private void OnDestroy()
    {
        Actions.OnSpellPanelToggle -= HandlePanelToggle;
    }

    void Start()
    {
        topPanel.localScale = Vector3.zero;
        botPanel.localScale = Vector3.zero;
    }

    public void HandleSaveButtonClick()
    {
        print("Saving a Level... ");
        SaveManager.Save();
    }

    public void HandleLoadButtonClick()
    {
        print("Loading a Level...");
        print(SaveManager.Load());
    }

    public void HandlePanelToggle(bool show)
    {
        if (show)
        {
            topPanel.localScale = Vector3.one;
            botPanel.localScale = Vector3.one;
        }
        else
        {
            topPanel.localScale = Vector3.zero;
            botPanel.localScale = Vector3.zero;
        }
    }
}
