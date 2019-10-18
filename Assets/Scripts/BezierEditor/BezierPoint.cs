using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierPoint
{
    public Vector3 position = Vector3.zero;

    public BezierPoint(Vector3 position)
    {
        this.position = position;
    }

    public static implicit operator Vector3(BezierPoint point)
    {
        return point.position;
    }
}
