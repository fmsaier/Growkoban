using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple fader changes the alpha of UI rect transform from 0f to 1f 
/// and vice-versa to create the fade in/out effect
/// </summary>
public class SimpleFader : MonoBehaviour
{
    [SerializeField, Tooltip("The container that holds image to use to change the alpha")]
    RectTransform faderRectXForm;

    [SerializeField, Tooltip("The UI image to use to change the alpha")]
    MaskableGraphic faderImage;
    MaskableGraphic FaderImage
    {
        get
        {
            if (faderImage == null)
                faderImage = GetComponent<MaskableGraphic>();
            return faderImage;
        }
    }


    void SetRaycastTargetState(bool isActive) => FaderImage.raycastTarget = isActive;

    /// <summary>
    /// Make it be 100% transperant 
    /// Disables the raycast target so that the player can click on things behind it
    /// </summary>
    public void Clear()
    {
        LeanTween.alpha(faderRectXForm, 0f, 0f);
        SetRaycastTargetState(false);
    }

    /// <summary>
    /// Make it be 100% solid 
    /// Enables the raycast target to prevent that the player from click on things behind it
    /// </summary>
    public void Blackout()
    {
        LeanTween.alpha(faderRectXForm, 1f, 0f);
        SetRaycastTargetState(true);
    }

    private void Awake() => Blackout();

    /// <summary>
    /// Fades from current alpha to 1f (solid color)
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator FadeOutRoutine(float time = 1f)
    {
        // Make sure the screen is clear before we try to fade
        Clear();
        yield return new WaitForSeconds(.1f);

        LeanTween.alpha(faderRectXForm, 1f, time);
        yield return new WaitForSeconds(time);

        Blackout();
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Fades from current alpha down to 0f (transperant)
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator FadeInRoutine(float time = 1f)
    {
        // Make sure the screen is a solid color before we try to fade
        Blackout();
        yield return new WaitForSeconds(.1f);

        LeanTween.alpha(faderRectXForm, 0f, time);
        yield return new WaitForSeconds(time);

        Clear();
        yield return new WaitForEndOfFrame();
    }
}