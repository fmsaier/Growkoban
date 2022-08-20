
public class HoleTile : Node
{
    public override bool IsWalkable
    {
        get
        {
            return !IsEmpty;
        }

        set
        {
            ;
        }
    }

    /// <summary>
    /// The hole has not been filled by a crate
    /// </summary>
    public bool IsEmpty { get; set; } = true;
}
