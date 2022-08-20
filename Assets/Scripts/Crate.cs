using UnityEngine;

public class Crate : Moveable
{
    [SerializeField]
    Sprite inHoleSprite;

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

    public override void FillHole()
    {
        SpriteRenderer.sprite = inHoleSprite;
        SpriteRenderer.sortingLayerID = SortingLayer.NameToID("Ground");
    }
}
