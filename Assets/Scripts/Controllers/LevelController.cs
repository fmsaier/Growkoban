using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

public class LevelController : Singleton<LevelController>
{
    [SerializeField]
    float moveTime = .3f;

    [SerializeField]
    LeanTweenType moveTween = LeanTweenType.linear;

    GridMap gridMap;
    GridMap GridMap
    {
        get
        {
            if (gridMap == null)
                gridMap = FindObjectOfType<GridMap>();
            return gridMap;
        }
    }

    [SerializeField]
    List<Player> activePlayers;
    List<Player> Players
    {
        get
        {
            if (activePlayers == null || activePlayers.Count < 1)
                activePlayers = FindObjectsOfType<Player>().ToList();
            return activePlayers;
        }
    }

    List<Player> movingPlayers;
    List<Player> MovingPlayers
    {
        get
        {
            if (movingPlayers == null)
                movingPlayers = new List<Player>();
            return movingPlayers;
        }
    }

    List<StarTile> starTiles;
    List<StarTile> StarTiles
    {
        get
        {
            if (starTiles == null || starTiles.Count < 1)
                starTiles = FindObjectsOfType<StarTile>().Where(s => !s.ignore).ToList();
            return starTiles;
        }
    }

    IEnumerator currentRoutine;
    bool CanRun
    {
        get
        {
            return IsLoaded && !GameManager.instance.IsTransitioning && currentRoutine == null && MovingPlayers.Count < 1;
        }
    }

    bool IsLoaded { get; set; }
    public bool CanMenuBeOpened { get { return CanRun; } }
    public bool PlayerIsOnNode(Node node) => Players.Where(p => p.Node == node).FirstOrDefault() != null;
    public Node GetNode(Vector2 position) => GridMap.GetNode(position);
    public void AddActivePlayer(Player player)
    {
        if (!Players.Contains(player))
            Players.Add(player);
    }
    public void RemoveActivePlayer(Player player)
    {
        if (Players.Contains(player))
            Players.Remove(player);
    }

    void CheckForDuplication(Player player, Vector2 direction)
    {
        var duplicator = player.Node as DuplicationTile;
        if (duplicator != null)
            duplicator.Duplicate(direction);
    }

    private void Start() => StartCoroutine(LoadLevelRoutine());

    IEnumerator LoadLevelRoutine()
    {
        IsLoaded = false;
        GridMap.Build();
        yield return StartCoroutine(GameManager.instance.FadeInRoutine());
        IsLoaded = true;
    }

    void Update()
    {
        if (IsLoaded && Keyboard.current.rKey.wasPressedThisFrame)
        {
            GameManager.instance.ReloadLevel();
            return;
        }

        if (!CanRun)
            return;

        var direction = MoveController.instance.GetMoveDirection();
        if (direction == Vector2.zero)
            return;

        // Sort the players so that the furthest is the direction we are moving is moved first
        // since it will determined if the one behind it, should there be one, can move
        var dir = Utilities.VectorToDirection(direction);
        List<Player> players = new List<Player>();        

        switch (dir)
        {
            case Direction.Up:
                players = Players.OrderByDescending(p => p.Position.y).ToList();
                break;
            case Direction.Right:
                players = Players.OrderByDescending(p => p.Position.x).ToList();
                break;
            case Direction.Down:
                players = Players.OrderBy(p => p.Position.y).ToList();
                break;
            case Direction.Left:
                players = Players.OrderBy(p => p.Position.x).ToList();
                break;
        }

        currentRoutine = HandleMovementRoutine(players, direction);
        StartCoroutine(currentRoutine);
    }    

    IEnumerator HandleMovementRoutine(List<Player> players, Vector2 direction)
    {
        // Clean up to be safe
        MovingPlayers.Clear();

        IEnumerator routine;
        Node node;
        foreach (var player in players)
        {
            node = GridMap.GetNode(player.Position + direction);

            // No where to move...bonk
            if (node == null)
                routine = BonkRoutine(player, direction);
            else
            {
                // Cannot move if another player is occupying that node
                var pNode = players.Where(p => p.Node == node).FirstOrDefault();
                if(pNode != null)
                    routine = BonkRoutine(player, direction);
                else
                    routine = MoveRoutine(player, node, direction);
            }

            MovingPlayers.Add(player);
            StartCoroutine(routine);

            // Wait a frame to allow time for us to change the player's current node
            // in case it affects the next player
            yield return new WaitForEndOfFrame();
        }

        // Wait until every one is done moving
        while (MovingPlayers.Count > 0)
            yield return new WaitForEndOfFrame();

        // If we have no more players have moving (probably fell into a hole)
        // then it is game over so reload
        if (Players.Count < 1)
            GameManager.instance.ReloadLevel();

        // All star tiles are occupied by the player, level completed
        else if(StarTiles.Where(s=> !s.HasPlayer).FirstOrDefault() == null)
            GameManager.instance.NextLevel();

        currentRoutine = null;
    }

    IEnumerator BonkRoutine(Player player, Vector2 direction)
    {
        yield return StartCoroutine(MoveController.instance.PlayerBonkRoutine(player, direction));
        if (MovingPlayers.Contains(player))
            MovingPlayers.Remove(player);
    }

    IEnumerator MoveRoutine(Player player, Node node, Vector2 direction)
    {
        // If the node is walkable then we can simply move into it
        if (node.IsWalkable)
        {
            // If the current node we are moving away from is a duplication tile
            // We want to trigger it to duplicate
            CheckForDuplication(player, direction);
            yield return StartCoroutine(MoveController.instance.MovePlayerRoutine(player, node, direction, moveTime));
        }
        else
        {
            var hole = node as HoleTile;

            // There is a crate we can push
            if (node.Crate != null)
                yield return StartCoroutine(PushCrateRoutine(player, node, direction));

            // Fall into empty hole
            else if(hole != null && hole.IsEmpty)
                yield return StartCoroutine(FallIntoHoleRoutine(player, node, direction));

            // Colliding with a wall
            else
                yield return StartCoroutine(MoveController.instance.PlayerBonkRoutine(player, direction));
        }

        if (MovingPlayers.Contains(player))
            MovingPlayers.Remove(player);
    }

    /// <summary>
    /// Player jumped into an empty hole
    /// </summary>
    /// <param name="player"></param>
    /// <param name="node"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    IEnumerator FallIntoHoleRoutine(Player player, Node node, Vector2 direction)
    {
        // Jump into the hole
        yield return StartCoroutine(MoveController.instance.MovePlayerRoutine(player, node, direction, moveTime));

        // Destroy the player
        if (MovingPlayers.Contains(player))
            MovingPlayers.Remove(player);

        RemoveActivePlayer(player);
        Destroy(player.gameObject);
    }

    /// <summary>
    /// Player is pushing a moveable object
    /// </summary>
    /// <param name="player"></param>
    /// <param name="node"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    IEnumerator PushCrateRoutine(Player player, Node node, Vector2 direction)
    {
        var crate = node.Crate;
        var newNode = GridMap.GetNode(crate.Position + direction);
        var wall = newNode as WallTile;
        
        // Cannot move the crate is the new node doesn't exist, is a wall, or has a crate on it
        if (newNode == null || wall != null || newNode.Crate != null)
            yield return StartCoroutine(MoveController.instance.PlayerBonkRoutine(player, direction));

        // Push the object and move
        else
            yield return StartCoroutine(MoveCrateRoutine(player, crate, node, newNode, direction));
    }

    /// <summary>
    /// Moving a moveable object and the player into an available node
    /// </summary>
    /// <param name="player"></param>
    /// <param name="crate"></param>
    /// <param name="playerNode"></param>
    /// <param name="crateNode"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    IEnumerator MoveCrateRoutine(Player player, Crate crate, Node playerNode, Node crateNode, Vector2 direction)
    {
        // If the current node we are moving away from is a duplication tile
        // We want to trigger it to duplicate
        CheckForDuplication(player, direction);

        // We need to move both the player and the moveable
        StartCoroutine(MoveController.instance.MoveToRoutine(crate.gameObject, crateNode.Position, moveTime));
        yield return StartCoroutine(MoveController.instance.PlayerPushRoutine(player, playerNode.Position, direction, moveTime));

        // Reassign the block to the new node
        playerNode.Crate = null;
        crateNode.Crate = crate;

        // Crate is covering a hole
        var hole = crateNode as HoleTile;
        if (hole && hole.IsEmpty)
        {
            crate.FillHole();
            hole.IsEmpty = false;

            // Since this crate filled in the hole
            // we will remove it from the node so it cannot be pushed out of the hole
            crateNode.Crate = null;
        }
    }
}
