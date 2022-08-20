using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : Node
{
    public override bool IsWalkable
    {
        get
        {
            return false;
        }

        set
        {
            base.IsWalkable = false;
        }
    }
}
