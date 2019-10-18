using UnityEngine;
using UnityEditor;
using System;
using static BezierLine.BezierPoints;

[CustomEditor(typeof(PolylineCreator))]
public class PolylineCreatorEditor : Editor
{
    PolylineCreator polylineCreator;


    public override void OnInspectorGUI()
    {        
        if (GUILayout.Button("Close line"))
        {
            polylineCreator.CloseLine();
        }

        if (GUILayout.Button("New line"))
        {
            polylineCreator.CreateStart();
        }

        if (GUILayout.Button("Export polyline"))
        {
            polylineCreator.ExportAsPolyline();
        }

        polylineCreator.drawLine = EditorGUILayout.Toggle("Draw", polylineCreator.drawLine);
    }

    void OnSceneGUI()
    {
        if(polylineCreator.start == null)
        {
            polylineCreator.CreateStart();
        }
        Input();
        if(polylineCreator.drawLine)
            Draw();
    }

    private void Draw()
    {
        //throw new NotImplementedException();

        BezierLine current = polylineCreator.start;
        do
        {
            Handles.DrawBezier(current.Start, current.End,
                current.StartHandle, current.EndHandle, Color.blue, null, 5f);

            Handles.color = Color.red;
            current.Start = Handles.FreeMoveHandle(current.Start, Quaternion.identity, .1f, Vector2.zero, Handles.SphereHandleCap);

            if (!polylineCreator.closed && current.nextLine == null)
            {
                current.End = Handles.FreeMoveHandle(current.End, Quaternion.identity, .1f, Vector2.zero, Handles.SphereHandleCap);
            }

            Handles.color = Color.green;
            current.StartHandle = Handles.FreeMoveHandle(current.StartHandle, Quaternion.identity, .1f, Vector2.zero, Handles.CubeHandleCap);
            current.EndHandle = Handles.FreeMoveHandle(current.EndHandle, Quaternion.identity, .1f, Vector2.zero, Handles.CubeHandleCap);
            Handles.color = Color.white;
            Handles.DrawDottedLine(current.Start, current.StartHandle, .5f);
            Handles.DrawDottedLine(current.End, current.EndHandle, .5f);

            current = current.nextLine;
        } while (current != polylineCreator.start && current != null);
    }

    static Plane XZPlane = new Plane(Vector3.up, Vector3.zero);

    private void Input()
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            // Undo.RecordObject(creator, "Add segment");
            float distance;
            Ray ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            if (XZPlane.Raycast(ray, out distance))
            {
                Vector3 hitPoint = ray.GetPoint(distance);
                //Just double check to ensure the y position is exactly zero
                hitPoint.y = 0;
                polylineCreator.AddLine(hitPoint);
            }
        }


    }

    void OnEnable()
    {
        polylineCreator = target as PolylineCreator;
    }
}