using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Direction Direction { get; protected set; }
    public bool IsJumping { get; set; }
    public Vector2 Position
    {
        get
        {
            return Utilities.RoundedPosition(transform);
        }
        set
        {
            transform.position = new Vector3(value.x, value.y, transform.position.z);
        }
    }

    /// <summary>
    /// The node the player is currently on
    /// </summary>
    public Node Node { get; set; }

    private void Start() => Node = LevelController.instance.GetNode(Position);

    public void SetDirection(Vector2 direction)
    {
        if (direction == Vector2.up)
            Direction = Direction.Up;

        if (direction == Vector2.right)
            Direction = Direction.Right;

        if (direction == Vector2.down)
            Direction = Direction.Down;

        if (direction == Vector2.left)
            Direction = Direction.Left;
    }
}
