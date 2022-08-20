using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("SFXs")]
    [SerializeField]
    SoundEffect hoverSfx;

    [SerializeField]
    SoundEffect clickSfx;

    [SerializeField]
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

    [SerializeField]
    TMP_Text levelNumberTxt;
    TMP_Text LevelNumberTxt
    {
        get
        {
            if (levelNumberTxt == null)
                levelNumberTxt = GetComponent<TMP_Text>();
            return levelNumberTxt;
        }
    }

    int levelNumber;
    public int LevelNumber 
    {
        get { return levelNumber; }
        set
        {
            // Levels start from 1 and up
            levelNumber = Mathf.Max(1, value);
            name = $"Level_{value}_btn";
            LevelNumberTxt.text = $"{value}";
        }
    }

    /// <summary>
    /// Configures the button based on the level data
    /// </summary>
    public void SetButtonLevel(int level)
    {
        LevelNumber = level;
        Button.interactable = true;
    }

    LevelSelect levelSelect;
    LevelSelect LevelSelect
    {
        get
        {
            if (levelSelect == null)
                levelSelect = FindObjectOfType<LevelSelect>();
            return levelSelect;
        }
    }

    private void Update()
    {
        // We want to hide the text that shows the level number when the button is not interactible
        // since there is a "lock" over the graphic that will be hidden by the numbers
        // LevelNumberTxt.enabled = Button.interactable;
    }

    public void OnPointerEnter(PointerEventData eventData) => PlaySfx(hoverSfx);
    public void OnPointerClick(PointerEventData eventData)
    {
        if (Button.interactable)
        {
            PlaySfx(clickSfx);
            LevelSelect.OnLevelSelected(LevelNumber);
        }   
    }

    /// <summary>
    /// Only when the button is interactible will play it
    /// </summary>
    /// <param name="sfx"></param>
    void PlaySfx(SoundEffect sfx)
    {
        if (Button.interactable)
            AudioManager.instance.Play(sfx);
    }

    public void Disable() => Button.interactable = false;
}
