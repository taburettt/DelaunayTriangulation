using System;
using System.Collections.Generic;
using UnityEngine;
using Graphs;

public static class Prim
{
    //public class Edge : Graphs.Edge
    //{
    //    public float Distance { get; private set; }

    //    public Edge(Vertex p1, Vertex p2) : base(p1, p2)
    //    {
    //        Distance = Vector2Int.Distance(p1.Position, p2.Position);
    //    }

    //    public static bool operator ==(Edge left, Edge right)
    //    {
    //        return (left.P1 == right.P1 && left.P2 == right.P2)
    //            || (left.P1 == right.P2 && left.P2 == right.P1);
    //    }

    //    public static bool operator !=(Edge left, Edge right)
    //    {
    //        return !(left == right);
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        if (obj is Edge e)
    //        {
    //            return this == e;
    //        }

    //        return false;
    //    }

    //    public bool Equals(Edge e)
    //    {
    //        return this == e;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return P1.GetHashCode() ^ P2.GetHashCode();
    //    }
    //}

    public static List<Edge> MinimumSpanningTree(List<Edge> edges, Vertex start)
    {
        HashSet<Vertex> openSet = new HashSet<Vertex>();
        HashSet<Vertex> closedSet = new HashSet<Vertex>();

        foreach (var edge in edges)
        {
            openSet.Add(edge.P1);
            openSet.Add(edge.P2);
        }

        closedSet.Add(start);

        List<Edge> results = new List<Edge>();

        while (openSet.Count > 0)
        {
            bool chosen = false;
            Edge chosenEdge = null;
            float minWeight = float.PositiveInfinity;

            foreach (var edge in edges)
            {
                int closedVertices = 0;
                if (!closedSet.Contains(edge.P1)) closedVertices++;
                if (!closedSet.Contains(edge.P2)) closedVertices++;
                if (closedVertices != 1) continue;

                if (edge.Distance < minWeight)
                {
                    chosenEdge = edge;
                    chosen = true;
                    minWeight = edge.Distance;
                }
            }

            if (!chosen) break;
            results.Add(chosenEdge);
            openSet.Remove(chosenEdge.P1);
            openSet.Remove(chosenEdge.P2);
            closedSet.Add(chosenEdge.P1);
            closedSet.Add(chosenEdge.P2);
        }

        return results;
    }
}