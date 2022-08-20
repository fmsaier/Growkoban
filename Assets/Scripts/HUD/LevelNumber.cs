using UnityEngine;
using TMPro;

public class LevelNumber : MonoBehaviour
{
    [SerializeField]
    TMP_Text text;
    TMP_Text Text
    {
        get
        {
            if (text == null)
                text = GetComponent<TMP_Text>();
            return text;
        }
    }

    void Start()
    {
        Text.text = $"Level {GameManager.instance.CurrentLevel}";           
    }
}
