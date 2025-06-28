using UnityEngine;

[CreateAssetMenu(fileName = "Level Detail #", menuName = "Scriptable Objects/Level Details", order = 1)]
public class soLevelDetails : ScriptableObject
{
    public string levelName;

    [Header("Fog")]
    public Color fogColor;
    public float fogIntensity;

    [Header("Light")]
    public Color ambientLightColor;

    [Header("Player")]
    public Vector3 playerStartLocation;
    public bool playRainEffect;
    public bool playSandstormEffect;

}
