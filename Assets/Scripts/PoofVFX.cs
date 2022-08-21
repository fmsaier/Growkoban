using UnityEngine;

public class PoofVFX : MonoBehaviour
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

    int poofAnimHash;
    private void Start()
    {
        poofAnimHash = Animator.StringToHash("Poof");
    }

    public void Poof() => Animator.Play(poofAnimHash);
}
