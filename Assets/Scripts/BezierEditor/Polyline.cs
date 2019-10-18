using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Polyline : ScriptableObject
{
    //ordered list of all nodes in the polyline
    public List<Vector3> nodes;

    public Polyline(List<Vector3> nodes)
    {
        this.nodes = nodes;
    }
}
