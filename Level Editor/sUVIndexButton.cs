using UnityEngine;

public class sUVIndexButton : MonoBehaviour
{
    private int _uvIndex;

    public void SetUVIndex(int newIndex)
    {
        _uvIndex = newIndex;
    }

    public void HandleButtonClick()
    {
        print("New UV Index: " + _uvIndex);
        Actions.OnLevelEditorUVIndexChange.Invoke(_uvIndex);
    }
}
