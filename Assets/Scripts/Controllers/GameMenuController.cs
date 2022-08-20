using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuController : Singleton<GameMenuController>
{
    [SerializeField]
    GameObject menuGO;

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

    bool MenuCanBeOpened { get { return LevelController.instance.CanMenuBeOpened; } }
    public bool MenuIsOpened { get { return menuGO.activeSelf; } }

    /// <summary>
    /// Register the inputs
    /// TODO: Do I need to make this part of UI instead of Player? probably
    /// </summary>
    void OnEnable()
    {
        InputActions.Player.Enable();
        InputActions.Player.Menu.performed += MenuInputAction;
    }

    /// <summary>
    /// Unregister since the object is gone and we don't want to cause undefined errors
    /// </summary>
    void OnDisable()
    {
        InputActions.Player.Menu.performed -= MenuInputAction;
    }

    void MenuInputAction(InputAction.CallbackContext obj)
    {
        // There are reasons to prevent opening the menu
        if (!MenuCanBeOpened)
            return;

        if (MenuIsOpened)
            CloseMenu();
        else
            StartCoroutine(OpenMenuRoutine());
    }

    public void CloseMenu()
    {
        menuGO.SetActive(false);
        Time.timeScale = 1f;
        AudioManager.instance.ResumeSFXs();
        Cursor.visible = false;
    }

    /// <summary>
    /// Stops time, shows the menu, and waits for the menu to be closed
    /// </summary>
    /// <returns></returns>
    IEnumerator OpenMenuRoutine()
    {
        Time.timeScale = 0f;
        menuGO.SetActive(true);
        AudioManager.instance.PauseSFXs();
        Cursor.visible = true;

        while (menuGO.activeSelf)
            yield return new WaitForEndOfFrame();
    }
}
