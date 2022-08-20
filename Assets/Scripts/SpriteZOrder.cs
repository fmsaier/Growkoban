using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteZOrder : MonoBehaviour
{
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
        SpriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.parent.position.y);
    }
}
