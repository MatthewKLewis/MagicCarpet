using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * 
 * Terrain Gen Editor
 * 
 */

public class LevelEditor : Editor
{
    [MenuItem("Terrain/Generate Terrain")]
    private static void GenerateTerrain()
    {
        Debug.Log("Hello from Level Editor");
        GameObject oneChunk = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Level Editor/LevelEditorChunk.prefab"));
    }
}