using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField]
    Sprite faceUpSprite;

    [SerializeField]
    Sprite jumpUpSprite;

    [SerializeField]
    Sprite faceRightSprite;

    [SerializeField]
    Sprite jumpRightSprite;

    [SerializeField]
    Sprite faceDownSprite;

    [SerializeField]
    Sprite jumpDownSprite;

    [SerializeField]
    Sprite faceLeftSprite;

    [SerializeField]
    Sprite jumpLeftSprite;

    [SerializeField]
    Player player;
    Player Player
    {
        get
        {
            if (player == null)
                player = GetComponentInParent<Player>();
            return player;
        }
    }

    [SerializeField]
    SpriteRenderer spriteRenderer;
    SpriteRenderer SpriteRenderer
    {
        get
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            return spriteRenderer;
        }
    }

    private void LateUpdate()
    {
        var sprite = SpriteRenderer.sprite;
        switch (Player.Direction)
        {
            case Direction.Up:
                sprite = Player.IsJumping ? jumpUpSprite : faceUpSprite;
                break;
            case Direction.Right:
                sprite = Player.IsJumping ? jumpRightSprite : faceRightSprite;
                break;
            case Direction.Down:
                sprite = Player.IsJumping ? jumpDownSprite : faceDownSprite;
                break;
            case Direction.Left:
                sprite = Player.IsJumping ? jumpLeftSprite : faceLeftSprite;
                break;
        }

        SpriteRenderer.sprite = sprite;
    }
}
