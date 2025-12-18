#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexMapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexMapGenerator generator = (HexMapGenerator)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Map"))
        {
            bool isRandomSeed = false;

            if (Application.isPlaying)
            {
                generator.RegenerateMap(isRandomSeed);
            }
            else
            {
                Debug.LogWarning("Entre em Play Mode para regenerar");
            }
        }

        if (GUILayout.Button("Generate Random Map"))
        {
            bool isRandomSeed = true;
            
            if (Application.isPlaying)
            {
                generator.RegenerateMap(isRandomSeed);
            }
            else
            {
                Debug.LogWarning("Entre em Play Mode para regenerar");
            }
        }
    }
}
#endif