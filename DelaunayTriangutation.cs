using System;
using System.Collections.Generic;
using UnityEngine;
using Graphs;

public class DelaunayTriangulation
{
    public class Triangle : IEquatable<Triangle>
    {
        public Vertex A { get; set; }
        public Vertex B { get; set; }
        public Vertex C { get; set; }
        public bool IsBad { get; set; }

        public Triangle()
        {

        }

        public Triangle(Vertex a, Vertex b, Vertex c)
        {
            A = a;
            B = b;
            C = c;
        }

        public bool ContainsVertex(Vector2Int v)
        {
            return Vector2Int.Distance(v, A.Position) < 0.01f
                || Vector2Int.Distance(v, B.Position) < 0.01f
                || Vector2Int.Distance(v, C.Position) < 0.01f;
        }

        public bool CircumCircleContains(Vector2Int v)
        {
            Vector2Int a = A.Position;
            Vector2Int b = B.Position;
            Vector2Int c = C.Position;

            int ab = a.sqrMagnitude;
            int cd = b.sqrMagnitude;
            int ef = c.sqrMagnitude;

            int circumX = (ab * (c.y - b.y) + cd * (a.y - c.y) + ef * (b.y - a.y)) / (a.x * (c.y - b.y) + b.x * (a.y - c.y) + c.x * (b.y - a.y));
            int circumY = (ab * (c.x - b.x) + cd * (a.x - c.x) + ef * (b.x - a.x)) / (a.y * (c.x - b.x) + b.y * (a.x - c.x) + c.y * (b.x - a.x));

            Vector2Int circum = new Vector2Int(circumX / 2, circumY / 2);
            Vector3 radVec = new Vector3(a.x, a.y) - new Vector3(circum.x, circum.y);
            float circumRadius = Vector3.SqrMagnitude(radVec);
            Vector3 distVec = new Vector3(v.x, v.y) - new Vector3(circum.x, circum.y);
            float dist = Vector3.SqrMagnitude(distVec);
            return dist <= circumRadius;
        }

        public static bool operator ==(Triangle left, Triangle right)
        {
            return (left.A == right.A || left.A == right.B || left.A == right.C)
                && (left.B == right.A || left.B == right.B || left.B == right.C)
                && (left.C == right.A || left.C == right.B || left.C == right.C);
        }

        public static bool operator !=(Triangle left, Triangle right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Triangle t)
            {
                return this == t;
            }

            return false;
        }

        public bool Equals(Triangle t)
        {
            return this == t;
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode();
        }
    }

    public static bool AlmostEqual(Vertex left, Vertex right)
    {
        return AlmostEqual(left.Position.x, right.Position.x) && AlmostEqual(left.Position.y, right.Position.y);
    }

    public static bool AlmostEqual(float x, float y)
    {
        return Mathf.Abs(x - y) <= float.Epsilon * Mathf.Abs(x + y) * 2
            || Mathf.Abs(x - y) < float.MinValue;
    }

    public List<Vertex> Vertices { get; private set; }
    public List<Edge> Edges { get; private set; }
    public List<Triangle> Triangles { get; private set; }

    public DelaunayTriangulation()
    {
        Edges = new List<Edge>();
        Triangles = new List<Triangle>();
        Vertices = new List<Vertex>();
    }

    public HashSet<Edge> Triangulate(List<Vertex> vertices)
    {
        Vertices = vertices;
        
        float minX = Vertices[0].Position.x;
        float minY = Vertices[0].Position.y;
        float maxX = minX;
        float maxY = minY;

        foreach (var vertex in Vertices)
        {
            if (vertex.Position.x < minX) minX = vertex.Position.x;
            if (vertex.Position.x > maxX) maxX = vertex.Position.x;
            if (vertex.Position.y < minY) minY = vertex.Position.y;
            if (vertex.Position.y > maxY) maxY = vertex.Position.y;
        }

        float dx = maxX - minX;
        float dy = maxY - minY;
        float deltaMax = Mathf.Max(dx, dy) * 2;

        Vertex p1 = new Vertex(new Vector2Int(Mathf.RoundToInt(minX - 1), Mathf.RoundToInt(minY - 1)));
        Vertex p2 = new Vertex(new Vector2Int(Mathf.RoundToInt(minX - 1), Mathf.RoundToInt(maxY + deltaMax)));
        Vertex p3 = new Vertex(new Vector2Int(Mathf.RoundToInt(maxX + deltaMax), Mathf.RoundToInt(minY - 1)));

        Triangles.Add(new Triangle(p1, p2, p3));

        foreach (var vertex in Vertices)
        {
            List<Edge> polygon = new List<Edge>();

            foreach (var t in Triangles)
            {
                if (t.CircumCircleContains(vertex.Position))
                {
                    t.IsBad = true;
                    polygon.Add(new Edge(t.A, t.B));
                    polygon.Add(new Edge(t.B, t.C));
                    polygon.Add(new Edge(t.C, t.A));
                }
            }

            Triangles.RemoveAll((Triangle t) => t.IsBad);

            for (int i = 0; i < polygon.Count; i++)
            {
                for (int j = i + 1; j < polygon.Count; j++)
                {
                    if (Edge.AlmostEqual(polygon[i], polygon[j]))
                    {
                        polygon[i].IsBad = true;
                        polygon[j].IsBad = true;
                    }
                }
            }

            polygon.RemoveAll((Edge e) => e.IsBad);

            foreach (var edge in polygon)
            {
                Triangles.Add(new Triangle(edge.P1, edge.P2, vertex));
            }
        }

        Triangles.RemoveAll((Triangle t) => t.ContainsVertex(p1.Position) || t.ContainsVertex(p2.Position) || t.ContainsVertex(p3.Position));

        HashSet<Edge> edgeSet = new HashSet<Edge>();

        foreach (var t in Triangles)
        {
            var ab = new Edge(t.A, t.B);
            var bc = new Edge(t.B, t.C);
            var ca = new Edge(t.C, t.A);

            if (edgeSet.Add(ab))
            {
                Edges.Add(ab);
            }

            if (edgeSet.Add(bc))
            {
                Edges.Add(bc);
            }

            if (edgeSet.Add(ca))
            {
                Edges.Add(ca);
            }
        }

        return edgeSet;
    }
}