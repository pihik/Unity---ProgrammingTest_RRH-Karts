using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PathGenerator : MonoBehaviour
{
    #region Singleton
    public static PathGenerator instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion
    [Tooltip("Needs to be in order.. first one is the starting point")]
    public RoadPart[] roadParts;
    [Tooltip("Height offset should be on same height as Kart on the road on Y position")]
    [SerializeField] float heightOffset = 1;

    public float HeightOffset { get { return heightOffset; } }

    public void GeneratePath()
    {

        for (int i = 0; i < roadParts.Length; i++) 
        {
            RoadPart nextRoadPart = (i == roadParts.Length - 1) ? roadParts[0] : roadParts[i + 1]; // if Roadpart is last one, his next roadpart is the first one
            RoadPart previousRoadPart = (i == 0) ? roadParts[roadParts.Length - 1] : roadParts[i - 1]; // if Roadpart is first one, his previous roadpart is the last one

            roadParts[i].SetRoadPart(nextRoadPart, previousRoadPart, heightOffset, i);
        }
    }
}

[CustomEditor(typeof(PathGenerator))]
public class PathGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathGenerator pathGenerator = (PathGenerator)target;

        if (GUILayout.Button("Generate Path"))
        {
            pathGenerator.GeneratePath();
            Debug.LogWarning("Path Generated");
        }
    }
}
