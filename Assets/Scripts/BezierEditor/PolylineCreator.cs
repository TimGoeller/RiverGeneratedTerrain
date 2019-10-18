using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

[Serializable]
public class PolylineCreator : MonoBehaviour
{
    public BezierLine start;
    public bool closed;
    public bool drawLine = true;
    public int exportResolution = 5;

    public void AddLine(Vector3 endPoint)
    {
        if (closed)
        {
            BezierLine current = start;
            float lowestMangitude = float.MaxValue;
            BezierLine currentClosest = current;

            do
            {
                float magnitude = ((endPoint - current.Start) + (endPoint - current.End)).magnitude;

                if (magnitude < lowestMangitude)
                {
                    lowestMangitude = magnitude;
                    currentClosest = current;
                }
                current = current.nextLine;
            }
            while (current.nextLine != start);

            BezierLine currentClosestNext = currentClosest.nextLine;
            BezierLine newLine = new BezierLine(currentClosest, endPoint);
            currentClosestNext.SetPreviousLine(newLine);
        }
        else
        {
            BezierLine currentLast = GetLastLine();
            BezierLine newLine = new BezierLine(currentLast, endPoint);
            newLine.SetPreviousLine(currentLast);
        }

    }

    public void CreateStart()
    {
        start = new BezierLine(transform.position, transform.position + Vector3.right);
        closed = false;
    }

    public BezierLine GetLastLine()
    {
        BezierLine current = start;
        while (current.nextLine != null)
        {
            current = current.nextLine;
        }

        return current;
    }


    public void CloseLine()
    {
        BezierLine lastLine = GetLastLine();
        BezierLine fillerLine = new BezierLine(lastLine.End, start.Start);
        fillerLine.SetPreviousLine(lastLine);
        start.SetPreviousLine(fillerLine);
        closed = true;
    }

    public void ExportAsPolyline()
    {
        List<Vector3> points = new List<Vector3>();

        BezierLine current = start;

        do
        {
            for (int i = 0; i < exportResolution; i++)
            {
                float step = ((float)i / exportResolution);
                Debug.Log(step);
                Vector3 pointA = Vector3.Lerp(current.Start, current.StartHandle, step);
                Vector3 pointB = Vector3.Lerp(current.StartHandle, current.EndHandle, step);
                Vector3 pointC = Vector3.Lerp(current.EndHandle, current.End, step);

                Vector3 pointAB = Vector3.Lerp(pointA, pointB, step);
                Vector3 pointBC = Vector3.Lerp(pointB, pointC, step);

                points.Add(Vector3.Lerp(pointAB, pointBC, step));
                Debug.Log(Vector3.Lerp(pointAB, pointBC, step).ToString());
            }

            current = current.nextLine;
        }
        while (current != start);

        Polyline polyline = ScriptableObject.CreateInstance<Polyline>();
        polyline.nodes = points;

        AssetDatabase.CreateAsset(polyline, "Assets/MyPolyline.asset");
    }

}

