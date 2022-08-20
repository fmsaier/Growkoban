using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    [SerializeField, Tooltip("The level selection buttons")]
    LevelButton buttonPrefab;

    bool loadingLevel;
    List<LevelButton> buttons;

    private void Start()
    {
        buttons = new List<LevelButton>();
        for (int i = 1; i <= GameManager.instance.TotalLevels; i++)
        {
            var button = Instantiate(buttonPrefab, transform);            
            button.SetButtonLevel(i);
            buttons.Add(button);
        }
    }

    public void OnLevelSelected(int level)
    {
        // Already doing it
        if (loadingLevel)
            return;

        // Disable all buttons
        buttons.ForEach(b => b.Disable());

        // If we have a MainMenu Controller then let's do the same
        //var menuController = FindObjectOfType<MainMenuController>();
        //if (menuController != null)
        //    menuController.DisableAllButtons();

        loadingLevel = true;
        GameManager.instance.LoadLevel(level);
    }
}
