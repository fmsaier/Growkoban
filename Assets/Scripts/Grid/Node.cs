using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node : MonoBehaviour
{
    Crate crate;
    public Crate Crate
    {
        get { return crate; }
        set
        {
            crate = value;
            IsWalkable = (crate == null);
        }
    }

    public virtual bool IsWalkable { get; set; } = true;
    public Vector2 Position 
    { 
        get 
        {
            return Utilities.RoundedPosition(transform);
        } 
    }
    public int x { get { return Mathf.RoundToInt(Position.x); } }
    public int y { get { return Mathf.RoundToInt(Position.y); } }
    
    Dictionary<Vector3, Node> neighbors;
    public Dictionary<Vector3, Node> Neighbors
    {
        get
        {
            if (neighbors == null)
                neighbors = new Dictionary<Vector3, Node>();
            return neighbors;
        }
    }

    [SerializeField]
    List<Node> nodes;
    public void ShowNeighbors() => nodes = Neighbors.Values.ToList();
}
