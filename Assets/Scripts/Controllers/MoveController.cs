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

    /// <summary>
    /// Bonks in the direction given
    /// </summary>
    /// <param name="player"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public IEnumerator PlayerBonkRoutine(Player player, Vector2 direction)
    {
        player.Walk(direction);
        yield return new WaitForSeconds(bonkTime);
        yield return new WaitForSeconds(bonkTime);
        player.Idle(direction);
    }

    /// <summary>
    /// Performs the jump or pop as it moves to the given destination
    /// </summary>
    /// <param name="player"></param>
    /// <param name="direction"></param>
    /// <param name="destination"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator MovePlayerRoutine(Player player, Vector2 destination, Vector2 direction, float time)
    {
        player.Jump(direction);

        LeanTween.move(player.gameObject, destination, time);
        yield return new WaitForSeconds(time);

        player.Idle(direction);
    }

    /// <summary>
    /// Player is pushing in a given direciton
    /// </summary>
    /// <param name="player"></param>
    /// <param name="destination"></param>
    /// <param name="direction"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator PlayerPushRoutine(Player player, Node destinationNode, Vector2 direction, float time)
    {
        player.Push(direction);
        player.Node = destinationNode;
        LeanTween.move(player.gameObject, destinationNode.Position, time);
        yield return new WaitForSeconds(time);
        player.Idle(direction);
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
        player.Node = node;
        yield return StartCoroutine(MovePlayerRoutine(player, node.Position, direction, time));
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
