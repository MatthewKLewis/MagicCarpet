using UnityEngine;

public class sLevelEditorHUD : MonoBehaviour
{
    LevelEditorManager lEM;

    [SerializeField] private Transform topPanel;
    [SerializeField] private Transform botPanel;

    private void Awake()
    {
        Actions.OnLevelEditorPanelToggle += HandlePanelToggle;
    }

    private void OnDestroy()
    {
        Actions.OnLevelEditorPanelToggle -= HandlePanelToggle;
    }

    void Start()
    {
        lEM = LevelEditorManager.instance;

        topPanel.localScale = Vector3.zero;
        botPanel.localScale = Vector3.zero;
    }

    public void HandleSaveButtonClick()
    {
        print("Saving a Level... ");
        lEM.SaveImageFromHeights();
        
    }

    public void HandleLoadButtonClick()
    {
        print("Loading a Level...");
        lEM.Redraw();
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
