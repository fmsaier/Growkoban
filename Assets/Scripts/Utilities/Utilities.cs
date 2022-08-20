using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class Utilities
{
    public static readonly List<Vector2> CardinalPoints = new List<Vector2>()
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left,
    };

    public static List<Vector2> AdjacentPoints(Vector2 origin)
    {
        var tiles = new List<Vector2>();
        foreach (var point in CardinalPoints)
            tiles.Add(origin + point);

        return tiles;
    }

    public static Vector2 RoundedPosition(Transform transform)
    {
        // We are working in ints not floats and Unity likes to show what looks like an int
        // on the editor but then when we cast it to an int it rounds to a different number
        // Using Mathf.RoundToInt has proven to provide more of the numbers we expect to see
        return new Vector2(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );
    }

    public static T[] ShuffleArray<T>(T[] array)
    {
        var rand = new System.Random(RandomNumbers.Seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int rIndex = rand.Next(i, array.Length);

            T tempItem = array[rIndex];

            array[rIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }

    public static Direction VectorToDirection(Vector2 vector)
    {
        if (vector == Vector2.up)
            return Direction.Up;

        if (vector == Vector2.right)
            return Direction.Right;

        if (vector == Vector2.down)
            return Direction.Down;

        if (vector == Vector2.left)
            return Direction.Left;

        return Direction.None;
    }
}
