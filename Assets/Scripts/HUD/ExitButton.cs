using UnityEngine;

/// <summary>
/// A simple button to exit out of the application
/// </summary>
public class ExitButton : MenuButton
{
    private void Awake()
    {
        // Disable usage on WebGL
        if (Application.platform == RuntimePlatform.WebGLPlayer || Application.isEditor)
            gameObject.SetActive(false);
    }

    protected override void TriggerButtonAction()
    {
        GameManager.instance.QuiteGame();
    }
}
