using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGraph : MonoBehaviour
{
    public int initialCandiateNodeCount = 3;
    public OutlineNode start;
    [HideInInspector]
    public LinkedList<OutlineNode> outlineNodes = new LinkedList<OutlineNode>();
    public List<OutlineNode> candidateNodes = new List<OutlineNode>();
    public Polyline polyline;

    public void UpdateOutline()
    {
        outlineNodes = new LinkedList<OutlineNode>();

        foreach (Vector3 nodePosition in polyline.nodes)
        {
            outlineNodes.AddLast(new OutlineNode(nodePosition));
        }

        CalculateConcavity();

        for (int i = 0; i < initialCandiateNodeCount; i++)
        {
            candidateNodes.Add(outlineNodes.OrderByDescending(node => node.concavity + node.distanceToNextCandidateNode).First());
            RecalculateDistancesToCandidateNodes();
        }     
    }

    public void RecalculateDistancesToCandidateNodes()
    {
        List<float> distances = new List<float>();
        foreach (OutlineNode nodeToCalculate in outlineNodes)
        {
            distances.Add(outlineNodes.Where(node => candidateNodes.Contains(node)).Min(node => (nodeToCalculate.position - node.position).magnitude));
        }
        distances = NormalizeList(distances);

        int num = 0;
        foreach (OutlineNode node in outlineNodes)
        {
            node.distanceToNextCandidateNode = distances[num];
            num++;
        }
    }

    public void CalculateConcavity()
    {
        List<float> innerAngles = new List<float>();
        List<float> outerAngles = new List<float>();

        for (LinkedListNode<OutlineNode> it = outlineNodes.First; it != null; it = it.Next)
        {
            float innerAngle = Vector3.Angle(PreviousNode(it).Value.position - it.Value.position, NextNode(it).Value.position - it.Value.position);
            if (!IsVectorTripletOrientationClockwise(PreviousNode(it).Value.position, it.Value.position, NextNode(it).Value.position))
            {
                innerAngle = (180 - innerAngle) + 180;
            }
            innerAngles.Add(innerAngle);

            float outerAngle = Vector3.Angle(PreviousNode(PreviousNode(it)).Value.position - it.Value.position, NextNode(NextNode(it)).Value.position - it.Value.position);
            if (!IsVectorTripletOrientationClockwise(PreviousNode(PreviousNode(it)).Value.position, it.Value.position, NextNode(NextNode(it)).Value.position))
            {
                outerAngle = (180 - outerAngle) + 180;
            }
            outerAngles.Add(outerAngle);
        }

        innerAngles = NormalizeList(innerAngles).ConvertAll(angle => 1 - angle);
        outerAngles = NormalizeList(outerAngles).ConvertAll(angle => 1 - angle);

        int num = 0;
        foreach (OutlineNode node in outlineNodes)
        {
            node.concavity = innerAngles[num] + outerAngles[num] * 0.5f;
            num++;
        }
    }

    LinkedListNode<OutlineNode> NextNode(LinkedListNode<OutlineNode> node)
    {
        if (node.Next == null)
            return outlineNodes.First;
        else
            return node.Next;
    }

    LinkedListNode<OutlineNode> PreviousNode(LinkedListNode<OutlineNode> node)
    {
        if (node.Previous == null)
            return outlineNodes.Last;
        else
            return node.Previous;
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
        public float distanceToNextCandidateNode;

        public OutlineNode(Vector3 position)
        {
            this.position = position;
        }
    }
}


