using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridMap : MonoBehaviour
{
    Node[,] map;

    public int Columns { get; protected set; }
    public int Rows { get; protected set; }

    public void Build()
    {
        BuildMap();
        SetNeighbors();
        SetCrates();
    }

    void BuildMap()
    {
        var nodes = FindObjectsOfType<Node>();

        // +1 since to compensate for zero-based counting
        Columns = nodes.Max(n => n.x) + 1;
        Rows = nodes.Max(n => n.y) + 1;

        map = new Node[Columns, Rows];
        foreach (var node in nodes)
        {
            // To help us idenitfy them easier/faster
            node.name = $"Node_{node.x}_{node.y}";
            map[node.x, node.y] = node;
        }
    }

    void SetNeighbors()
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                var node = map[x, y];
                if (node == null)
                    continue;

                foreach (var point in Utilities.CardinalPoints)
                {
                    var neighborPos = node.Position + point;
                    var neighbor = GetNode(neighborPos);
                    node.Neighbors[point] = neighbor;
                }

                // So that it is easier to see which neighbors were calculated
                node.ShowNeighbors();
            }
        }
    }

    void SetCrates()
    {
        foreach (var crate in FindObjectsOfType<Crate>())
        {
            var node = GetNode(crate.Position);
            if (node != null)
                node.Crate = crate;
        }
    }

    public T GetNode<T>(Vector2 position) where T: MonoBehaviour
    {
        var node = GetNode(position);
        return node != null ? node.GetComponent<T>() : null;
    }

    public Node GetNode(Vector2 position)
    {
        return GetNode((int)position.x, (int)position.y);
    }    

    public Node GetNode(int x, int y)
    {
        var xInBounds = x >= 0f && x < map.GetLength(0);
        var yInBounds = y >= 0f && y < map.GetLength(1);
        return xInBounds && yInBounds ? map[x, y] : null;
    }
}
