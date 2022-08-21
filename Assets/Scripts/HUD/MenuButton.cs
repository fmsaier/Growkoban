using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField]
    MenuButtonType buttonType;

    [SerializeField]
    SoundEffect mouseOverSfx;

    [SerializeField]
    SoundEffect clickedSfx;

    Button button;
    Button Button 
    { 
        get 
        {
            if (button == null)
                button = GetComponent<Button>();
            return button;
        } 
    }

    public void Disable() => Button.interactable = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!Button.interactable)
            return;

        AudioManager.instance.Play(clickedSfx);
        TriggerButtonAction();
    }

    protected virtual void TriggerButtonAction()
    {
        MainMenuController.instance.OnMenuButtonPressed(buttonType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(Button.interactable)
            AudioManager.instance.Play(mouseOverSfx);
    }
}

public enum MenuButtonType
{
    LevelSelect,
    HowTo,
    Settings,
    Credits,
    Back,
    Exit
}