using UnityEngine;

public abstract class Moveable : MonoBehaviour
{
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

    public abstract void FillHole();
}
