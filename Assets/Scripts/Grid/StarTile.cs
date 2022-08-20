using UnityEngine;

public class StarTile : Node
{
    GameObject player;
    public bool HasPlayer { get { return player != null; } }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (player == null && collision.CompareTag("Player"))
            player = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player == collision.gameObject)
            player = null;
    }
}
