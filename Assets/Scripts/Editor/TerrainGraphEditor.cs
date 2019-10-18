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
        EditorGUILayout.LabelField("Import Resolution");
        terrainGraph.initialCandiateNodeCount = EditorGUILayout.IntSlider(terrainGraph.initialCandiateNodeCount, 1, 5);
        EditorGUI.BeginChangeCheck();
        terrainGraph.polyline = (Polyline)EditorGUILayout.ObjectField(terrainGraph.polyline, typeof(Polyline), false);
        if(EditorGUI.EndChangeCheck())
        {
            terrainGraph.UpdateOutline();
        }
    }

    public void OnSceneGUI()
    {
        Draw();
    }

    private void Draw()
    {
        int num = 0;
        foreach (TerrainGraph.OutlineNode point in terrainGraph.outlineNodes)
        {
            //if (point.IsNodeConcave())
            //{
            //    Handles.color = Color.red;
            //}
            //else
            //{
            //    Handles.color = Color.blue;
            //}
            Handles.color = Color.red;
            if(terrainGraph.candidateNodes.Contains(point))
            {
                Handles.color = Color.blue;
            }

            Handles.SphereHandleCap(0, point.position, Quaternion.identity, .2f, EventType.Repaint);
            Handles.Label(point.position, num.ToString());
            num++;
        }
    }

    void OnEnable()
    {
        terrainGraph = target as TerrainGraph;
    }
}
