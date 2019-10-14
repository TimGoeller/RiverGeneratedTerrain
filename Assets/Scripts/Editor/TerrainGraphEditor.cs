using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(TerrainGraph))]
public class TerrainGraphEditor : Editor
{

    public TerrainGraph terrainGraph;

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Import polyline"))
        {
            terrainGraph.ImportPolyline();
        }
        EditorGUILayout.LabelField("Import Resolution");
        terrainGraph.importResolution = EditorGUILayout.IntSlider(terrainGraph.importResolution, 1, 15);
        terrainGraph.initialCandiateNodeCount = EditorGUILayout.IntSlider(terrainGraph.initialCandiateNodeCount, 1, 5);
    }

    public void OnSceneGUI()
    {
        Draw();
    }

    private void Draw()
    {
        if(terrainGraph.start != null)
        {
            int num = 0;
            foreach(TerrainGraph.OutlineNode point in terrainGraph.GetOutlineEnumerator())
            {
                //if(point.IsNodeConcave())
                //{
                //    Handles.color = Color.red;
                //}
                //else
                //{
                //    Handles.color = Color.blue;
                //}
                Handles.SphereHandleCap(0, point.position, Quaternion.identity, .2f, EventType.Repaint);
                Handles.Label(point.position, num.ToString());
                num++;
            }
        }
    }

    void OnEnable()
    {
        terrainGraph = target as TerrainGraph;
    }
}
