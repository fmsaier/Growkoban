using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    Animator Animator
    {
        get
        {
            if (animator == null)
                animator = GetComponentInParent<Animator>();
            return animator;
        }
    }

    [Header("SFX")]
    [SerializeField] SoundEffect jumpSFX;
    [SerializeField] SoundEffect fallSFX;
    [SerializeField] SoundEffect pushSFX;

    int IdleDownAnimHash;
    int IdleUpAnimHash;
    int IdleLeftAnimHash;
    int IdleRightAnimHash;

    int JumpDownAnimHash;
    int JumpUpAnimHash;
    int JumpLeftAnimHash;
    int JumpRightAnimHash;

    int WalkDownAnimHash;
    int WalkUpAnimHash;
    int WalkLeftAnimHash;
    int WalkRightAnimHash;

    int PushDownAnimHash;
    int PushUpAnimHash;
    int PushLeftAnimHash;
    int PushRightAnimHash;

    int FallAnimHash;

    private void Start()
    {
        IdleUpAnimHash = Animator.StringToHash("IdleUp");
        IdleDownAnimHash = Animator.StringToHash("IdleDown");
        IdleLeftAnimHash = Animator.StringToHash("IdleLeft");
        IdleRightAnimHash = Animator.StringToHash("IdleRight");

        JumpUpAnimHash = Animator.StringToHash("JumpUp");
        JumpDownAnimHash = Animator.StringToHash("JumpDown");
        JumpLeftAnimHash = Animator.StringToHash("JumpLeft");
        JumpRightAnimHash = Animator.StringToHash("JumpRight");

        WalkUpAnimHash = Animator.StringToHash("WalkUp");
        WalkDownAnimHash = Animator.StringToHash("WalkDown");
        WalkLeftAnimHash = Animator.StringToHash("WalkLeft");
        WalkRightAnimHash = Animator.StringToHash("WalkRight");

        PushUpAnimHash = Animator.StringToHash("PushUp");
        PushDownAnimHash = Animator.StringToHash("PushDown");
        PushLeftAnimHash = Animator.StringToHash("PushLeft");
        PushRightAnimHash = Animator.StringToHash("PushRight");

        FallAnimHash = Animator.StringToHash("Fall");
    }

    public void Idle(Direction direction)
    {
        var state = FallAnimHash;
        switch (direction)
        {
            case Direction.Up:
                state = IdleUpAnimHash;
                break;
            case Direction.Right:
                state = IdleRightAnimHash;
                break;
            case Direction.Down:
                state = IdleDownAnimHash;
                break;
            case Direction.Left:
                state = IdleLeftAnimHash;
                break;
        }

        // Play it if not already playing it
        PlayAnimation(state);
    }

    public void Walk(Direction direction)
    {
        var state = FallAnimHash;
        switch (direction)
        {
            case Direction.Up:
                state = WalkUpAnimHash;
                break;
            case Direction.Right:
                state = WalkRightAnimHash;
                break;
            case Direction.Down:
                state = WalkDownAnimHash;
                break;
            case Direction.Left:
                state = WalkLeftAnimHash;
                break;
        }

        // Play it if not already playing it
        PlayAnimation(state);
    }

    public void Push(Direction direction)
    {
        var state = FallAnimHash;
        switch (direction)
        {
            case Direction.Up:
                state = PushUpAnimHash;
                break;
            case Direction.Right:
                state = PushRightAnimHash;
                break;
            case Direction.Down:
                state = PushDownAnimHash;
                break;
            case Direction.Left:
                state = PushLeftAnimHash;
                break;
        }

        // Play it if not already playing it
        AudioManager.instance.Play(pushSFX);
        PlayAnimation(state);
    }

    public void Jump(Direction direction)
    {
        var state = FallAnimHash;
        switch (direction)
        {
            case Direction.Up:
                state = JumpUpAnimHash;
                break;
            case Direction.Right:
                state = JumpRightAnimHash;
                break;
            case Direction.Down:
                state = JumpDownAnimHash;
                break;
            case Direction.Left:
                state = JumpLeftAnimHash;
                break;
        }

        // Play it if not already playing it
        AudioManager.instance.Play(jumpSFX);
        PlayAnimation(state);
    }

    public SFXAudioSource Fall()
    {
        PlayAnimation(FallAnimHash);
        return AudioManager.instance.Play(fallSFX);
    }
    void PlayAnimation(int state) => Animator.Play(state);
}
