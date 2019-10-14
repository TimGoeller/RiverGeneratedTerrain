using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PolylineCreator))]
public class TerrainGraph : MonoBehaviour
{
    public int importResolution;
    public int initialCandiateNodeCount = 3;
    public OutlineNode start;

    public void ImportPolyline()
    {
        List<Vector3> polylineVec = GetComponent<PolylineCreator>().GetPolyline(5);

        OutlineNode lastNode = null;
        start = null;

        foreach (Vector3 node in polylineVec)
        {
            OutlineNode newNode = new OutlineNode(node);

            if (start == null)
            {
                start = newNode;
            }

            if (lastNode != null)
            {
                newNode.SetPreviousNode(lastNode);
            }
            lastNode = newNode;
        }
        start.SetPreviousNode(lastNode);

        CalculateConcavity();
    }

    public void CalculateConcavity()
    {
        List<float> innerAngles = new List<float>();
        List<float> outerAngles = new List<float>();
        foreach (OutlineNode node in GetOutlineEnumerator())
        {
            float innerAngle = Vector3.Angle(node.previousNode.position - node.position, node.nextNode.position - node.position);
            if (!IsVectorTripletOrientationClockwise(node.previousNode.position, node.position, node.nextNode.position))
            {
                innerAngle = (180 - innerAngle) + 180;
            }
            innerAngles.Add(innerAngle);

            float outerAngle = Vector3.Angle(node.previousNode.previousNode.position - node.position, node.nextNode.nextNode.position - node.position);
            if (!IsVectorTripletOrientationClockwise(node.previousNode.previousNode.position, node.position, node.nextNode.nextNode.position))
            {
                outerAngle = (180 - outerAngle) + 180;
            }
            outerAngles.Add(outerAngle);
        }
        innerAngles = NormalizeList(innerAngles);
        outerAngles = NormalizeList(outerAngles);

        int num = 0;
        foreach (OutlineNode node in GetOutlineEnumerator())
        {
            node.concavity = innerAngles[num] + outerAngles[num] * 0.5f;
            num++;
        }
    }

    public List<float> NormalizeList(List<float> listToNormalize)
    {
        float max = listToNormalize.Max();
        float min = listToNormalize.Min();

        if(max == min)
        {
            return listToNormalize.Select(value =>
            {
                return 1f;
            }).ToList();
        }

        return listToNormalize.Select(value =>
        {
            return (value - min) / (max - min);
        }).ToList();
    }

    public IEnumerable GetOutlineEnumerator()
    {
        OutlineNode current = start;
        do
        {
            yield return current;
            current = current.nextNode;
        } while (current != start);        
    }

    public bool IsVectorTripletOrientationClockwise(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float val = (v2.z - v1.z) * (v3.x - v2.x) -
              (v2.x - v1.x) * (v3.z - v2.z);

        return (val >= 0) ? false : true;
    }

    public class OutlineNode
    {
        public Vector3 position;
        public float concavity;
        public OutlineNode previousNode;
        public OutlineNode nextNode;

        public OutlineNode(Vector3 position)
        {
            this.position = position;
        }

        public void SetPreviousNode(OutlineNode previous)
        {
            previousNode = previous;
            previous.nextNode = this;
        }

    }
}


