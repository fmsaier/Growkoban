using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : Singleton<MoveController>
{
    [SerializeField]
    float bonkDistance = .25f;

    [SerializeField]
    float hopHeight = .25f;

    [SerializeField]
    float hopScale = 1.1f;

    [SerializeField]
    float bonkTime = .1f;

    PlayerInputActions inputActions;
    PlayerInputActions InputActions
    {
        get
        {
            if (inputActions == null)
                inputActions = new PlayerInputActions();
            return inputActions;
        }
    }

    Vector2 lastFrameInput;
    Vector2 lastFrameDirection;
    bool movePressed;

    private void OnEnable()
    {
        InputActions.Player.Enable();
    }

    private void OnDisable()
    {
        InputActions.Player.Disable();
    }

    private void LateUpdate()
    {
        var playerInput = InputActions.Player.Movement.ReadValue<Vector2>();
        if (playerInput == Vector2.zero)
            movePressed = false;
    }

    public Vector2 GetMoveDirection()
    {
        // We want the player to "release" the button before 
        if (movePressed)
            return Vector2.zero;

        movePressed = true;
        var playerInput = InputActions.Player.Movement.ReadValue<Vector2>();

        // Rounding because we want to work in whole numbers for the "node" calculations to work
        // Seen some input where the value is a fraction like "0.71f" and not "1f"
        var direction = new Vector2(Mathf.RoundToInt(playerInput.x), Mathf.RoundToInt(playerInput.y));

        // Player input hasn't change therefore the direction should be the same as last frame
        if (lastFrameInput == playerInput)
            direction = lastFrameDirection;

        else if (direction.x != 0f && direction.y != 0f)
        {
            // Player inputs has changed and they are pressing both horizontal and vertical directions
            // Let's figure which one to move to

            // Previously not moving before so we will prioritize horizontal
            if (lastFrameDirection == Vector2.zero)
                direction.y = 0f;

            // Previously moving horizontally so let's move vertically
            if (lastFrameDirection.x != 0f)
                direction.x = 0f;

            // Previously moving vertically so let's move horizontally
            if (lastFrameDirection.y != 0f)
                direction.y = 0f;
        }

        // Save the input to know when the player's input has changed
        lastFrameInput = playerInput;

        // Save the direction we are going to move to handle player input change next frame
        lastFrameDirection = direction;

        // No input given
        if (direction == Vector2.zero)
        {
            // Save the direction since it is zero
            lastFrameDirection = direction;
        }

        return direction;
    }

    bool IsHorizontalDirection(Vector2 direction)
    {
        return (direction == Vector2.left || direction == Vector2.right);
    }

    /// <summary>
    /// Based on the direction moving will be how much we will move to simulate the bonk
    /// </summary>
    /// <param name="player"></param>
    /// <param name="destination"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    Vector2 GetBonkDestination(Player player, Vector3 destination, Vector2 direction)
    {
        bool horizontalMove = IsHorizontalDirection(direction);

        // Cut the distance so that it looks like we bonk
        if (horizontalMove)
            destination.x = player.Position.x + (bonkDistance * direction.x);
        else
            destination.y = player.Position.y + (bonkDistance * direction.y);
        return destination;
    }      

    /// <summary>
    /// Bonks in the direction given
    /// </summary>
    /// <param name="player"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public IEnumerator PlayerBonkRoutine(Player player, Vector2 direction)
    {
        var origin = player.Position;
        var destination = GetBonkDestination(player, player.Position + direction, direction);

        // Move into destination
        yield return StartCoroutine(MovePlayerRoutine(player, destination, direction, bonkTime));

        // Bonk back to origin        
        yield return StartCoroutine(BonkPlayerBackToOriginRoutine(player, origin));
    }

    IEnumerator BonkPlayerBackToOriginRoutine(Player player, Vector2 origin)
    {
        // Play Bonk Sound
        LeanTween.move(player.gameObject, origin, bonkTime);
        yield return new WaitForSeconds(bonkTime);
    }

    /// <summary>
    /// Performs the jump or pop as it moves to the given destination
    /// </summary>
    /// <param name="player"></param>
    /// <param name="direction"></param>
    /// <param name="destination"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator MovePlayerRoutine(Player player, Vector2 direction, Vector2 destination, float time)
    {
        player.IsJumping = true;
        player.SetDirection(direction);

        // Do a Hop or Pop        
        if (IsHorizontalDirection(direction))
            yield return StartCoroutine(PlayerJumpToDestinationRoutine(player, destination, time));
        else
            yield return StartCoroutine(PlayerPopToDestinationRoutine(player, destination, time));

        player.IsJumping = false;
        player.SetDirection(direction);
    }

    /// <summary>
    /// Assing the given node as the new node where the player is at and moves the player into the given node
    /// </summary>
    /// <param name="player"></param>
    /// <param name="node"></param>
    /// <param name="direction"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator MovePlayerRoutine(Player player, Node node, Vector2 direction, float time)
    {
        // Since the player is moving into a new node
        // we need to update their current node so that everyone else is aware of where the player is at
        player.Node = node;
        yield return StartCoroutine(MovePlayerRoutine(player, node.Position, direction, time));
    }

    /// <summary>
    /// Plays the player's jump animation as it moves into the destination
    /// </summary>
    /// <param name="player"></param>
    /// <param name="destination"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator PlayerJumpToDestinationRoutine(Player player, Vector2 destination, float time)
    {
        // Start Moving
        var halfTime = time / 2;
        LeanTween.moveX(player.gameObject, destination.x, time);

        // Jump Up
        LeanTween.moveY(player.gameObject, player.Position.y + hopHeight, halfTime);
        yield return new WaitForSeconds(halfTime);

        // Land
        LeanTween.moveY(player.gameObject, destination.y, halfTime);
        yield return new WaitForSeconds(halfTime);
    }

    /// <summary>
    /// Plays a "pop" animation as it moves into the tile
    /// </summary>
    /// <param name="player"></param>
    /// <param name="destination"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator PlayerPopToDestinationRoutine(Player player, Vector2 destination, float time)
    {
        // Scale up
        var halfTime = time / 2;
        var scale = new Vector3(hopScale, hopScale, 1f);
        LeanTween.scale(player.gameObject, scale, halfTime);

        // Start Moving
        LeanTween.move(player.gameObject, destination, time);
        yield return new WaitForSeconds(halfTime);

        // Scale down
        LeanTween.scale(player.gameObject, Vector3.one, halfTime);
        yield return new WaitForSeconds(halfTime);
    }

    /// <summary>
    /// Moves the given object into the given destination
    /// </summary>
    /// <param name="go"></param>
    /// <param name="destination"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator MoveToRoutine(GameObject go, Vector2 destination, float time, LeanTweenType ease = LeanTweenType.linear)
    {
        LeanTween.move(go, destination, time).setEase(ease);
        yield return new WaitForSeconds(time);
    }
}
