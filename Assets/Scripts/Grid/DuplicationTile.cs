using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicationTile : Node
{
    [SerializeField]
    bool allowRespawn;

    [SerializeField]
    Player playerPrefab;

    [SerializeField]
    Sprite avatarSprite;

    [SerializeField]
    Sprite outlineSprite;

    [SerializeField]
    SoundEffect poofSFX;

    [SerializeField]
    PoofVFX poofVFX;
    PoofVFX PoofVFX
    {
        get
        {
            if (poofVFX == null)
                poofVFX = GetComponentInChildren<PoofVFX>();
            return poofVFX;
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

    [SerializeField]
    Player spawnedPlayer;

    IEnumerator routine;
    bool CanMultiply { get; set; } = true;

    private void Start() => CanMultiply = (spawnedPlayer == null);

    private void LateUpdate()
    {
        if(allowRespawn)
            CanMultiply = spawnedPlayer == null;

        var sprite = CanMultiply ? avatarSprite : outlineSprite;
        SpriteRenderer.sprite = sprite;
    }

    public void Duplicate(Vector2 faceDirection)
    {
        // Already in use
        if (!CanMultiply || routine != null)
            return;

        routine = MutliplyRoutine(faceDirection);
        StartCoroutine(routine);
    }

    IEnumerator MutliplyRoutine(Vector2 faceDirection)
    {
        // While there is something on this node
        // We cannot spawn the new player
        while (!IsWalkable || Crate != null || LevelController.instance.PlayerIsOnNode(this))
            yield return new WaitForEndOfFrame();

        // Poof
        PoofVFX.Poof();
        var src = AudioManager.instance.Play(poofSFX);
        yield return new WaitForEndOfFrame();

        // Spawning and waiting a frame
        spawnedPlayer = Instantiate(playerPrefab);
        spawnedPlayer.transform.position = transform.position;
        spawnedPlayer.SetDirection(faceDirection);
        LevelController.instance.AddActivePlayer(spawnedPlayer);
        yield return null;

        // Remove this when we support re-using a duplication tile
        CanMultiply = false;
        routine = null;
    }
}
