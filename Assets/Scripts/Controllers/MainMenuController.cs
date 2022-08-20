using UnityEngine;

/// <summary>
/// A very simplistic turn objects ON/OFF to simulate switching menus
/// Does not handle animations of any kind
/// </summary>
public class MainMenuController : Singleton<MainMenuController>
{
    [SerializeField]
    GameObject levelSelectMenu;

    [SerializeField]
    GameObject howToPlayMenu;

    [SerializeField]
    GameObject settingsMenu;

    [SerializeField]
    GameObject creditsMenu;

    GameObject currentMenu;

    private void Start()
    {
        GameManager.instance.FadeIn();
        GameManager.instance.SetMouseVisiblity(true);

        OnSceneLoaded();
    }

    void OnSceneLoaded()
    {
        CloseMenu(howToPlayMenu);
        CloseMenu(settingsMenu);
        CloseMenu(creditsMenu);

        OpenMenu(levelSelectMenu);
    }

    public void OnMenuButtonPressed(MenuButtonType buttonType)
    {
        switch (buttonType)
        {
            case MenuButtonType.LevelSelect:
                //// Disable all the buttons to prevent them from being accessible
                //foreach (var button in FindObjectsOfType<MenuButton>())
                //    button.Disable();

                //GameManager.instance.NextLevel();
                OpenMenu(levelSelectMenu);
                break;
            case MenuButtonType.HowTo:
                OpenMenu(howToPlayMenu);
                break;
            case MenuButtonType.Settings:
                OpenMenu(settingsMenu);
                break;
            case MenuButtonType.Credits:
                OpenMenu(creditsMenu);
                break;
            case MenuButtonType.Back:
                CloseMenu(currentMenu);
                OpenMenu(levelSelectMenu);
                break;
        }
    }

    public void DisableAllButtons()
    {
        foreach (var button in FindObjectsOfType<MenuButton>())
            button.Disable();
    }

    void OpenMenu(GameObject menuGO)
    {
        CloseMenu(currentMenu);
        currentMenu = menuGO;
        currentMenu.SetActive(true);
    }

    void CloseMenu(GameObject menu)
    {
        if (menu != null)
            menu.SetActive(false);
    }
}
