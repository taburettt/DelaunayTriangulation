using System;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs
{
    public class Vertex : IEquatable<Vertex>
    {
        public Vector2Int Position { get; private set; }

        public Vertex()
        {

        }

        public Vertex(Vector2Int position)
        {
            Position = position;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vertex v)
            {
                return Position == v.Position;
            }

            return false;
        }

        public bool Equals(Vertex other)
        {
            return Position == other.Position;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }

    public class Vertex<T> : Vertex
    {
        public T Item { get; private set; }

        public Vertex(T item)
        {
            Item = item;
        }

        public Vertex(Vector2Int position, T item) : base(position)
        {
            Item = item;
        }
    }

    public class Edge : IEquatable<Edge>
    {
        public Vertex P1 { get; set; }
        public Vertex P2 { get; set; }
        public bool IsBad { get; set; }
        public float Distance { get; private set; }

        public Edge()
        {

        }

        public Edge(Vertex p1, Vertex p2)
        {
            P1 = p1;
            P2 = p2;
            Distance = Vector2Int.Distance(p1.Position, p2.Position);
        }

        public static bool operator ==(Edge left, Edge right)
        {
            return (left.P1 == right.P1 || left.P1 == right.P2)
                && (left.P2 == right.P1 || left.P2 == right.P2);
        }

        public static bool operator !=(Edge left, Edge right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Edge e)
            {
                return this == e;
            }

            return false;
        }

        public bool Equals(Edge e)
        {
            return this == e;
        }

        public override int GetHashCode()
        {
            return P1.GetHashCode() ^ P2.GetHashCode();
        }

        public static bool AlmostEqual(Edge left, Edge right)
        {
            return DelaunayTriangulation.AlmostEqual(left.P1, right.P1) && DelaunayTriangulation.AlmostEqual(left.P2, right.P2)
                || DelaunayTriangulation.AlmostEqual(left.P1, right.P2) && DelaunayTriangulation.AlmostEqual(left.P2, right.P1);
        }
    }
}