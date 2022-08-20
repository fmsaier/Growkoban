using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{
    [SerializeField, Tooltip("The prefix that all level have for a scene name")]
    string levelNamePrefix = "Level_";

    [SerializeField, Tooltip("True: Make all levels accessible from the start")]
    bool enableAllLevels = false;

    public bool GameOver { get; set; }
    public bool GamePaused { get; set; }
    public bool IsTransitioning { get; set; }

    int totalLevels;
    public int TotalLevels
    {
        get
        {
            if (totalLevels < 1)
            {
                for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
                {
                    string sceneName = $"{levelNamePrefix}{i}";
                    if (Application.CanStreamedLevelBeLoaded(sceneName))
                        totalLevels++;
                }
            }
            return totalLevels;
        }
    }

    public int CurrentLevel
    {
        get
        {
            var level = 0;
            string sceneNumber = Regex.Match(CurrentSceneName, @"\d+").Value;
            if (!string.IsNullOrEmpty(sceneNumber))
                level = int.Parse(sceneNumber);

            return level;
        }
    }

    public string CurrentSceneName
    {
        get
        {
            return SceneManager.GetActiveScene().name;
        }
    }

    Player player;
    public Player Player
    {
        get
        {
            if (player == null)
                player = FindObjectOfType<Player>();
            return player;
        }
    }

    SimpleFader fader;
    SimpleFader Fader
    {
        get
        {
            if (fader == null)
                fader = FindObjectOfType<SimpleFader>();
            return fader;
        }
    }

    void Start()
    {
        if (gameObject == null)
            return;

        // Play the music
        AudioManager.instance.StartMusic();
    }

    void Update()
    {
        // Support escape to quit the app
        // Except for mobile and webgl builds
        if (!Application.isMobilePlatform && !Application.isEditor && Application.platform != RuntimePlatform.WebGLPlayer && Keyboard.current.escapeKey.wasPressedThisFrame)
            QuiteGame();
    }

    public void QuiteGame() => Application.Quit();      

    /// <summary>
    /// Sets the cursor's visibility
    /// </summary>
    /// <param name="isVisible"></param>
    public void SetMouseVisiblity(bool isVisible) => Cursor.visible = isVisible;

    /// <summary>
    /// Transitions into the curent level 
    /// </summary>
    public void ReloadLevel() => LoadLevel(SceneManager.GetActiveScene().buildIndex);
    
    /// <summary>
    /// Transitions into the next level
    /// If the next level is not part of the build, it loads the first scene on the build
    /// </summary>
    public void NextLevel()
    {
        var buildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        LoadLevel(buildIndex);
    }

    /// <summary>
    /// Attempts to fade into the given build index
    /// When the build index is out of range, it defaults to the first scene on the build
    /// </summary>
    /// <param name="buildIndex"></param>
    public void LoadLevel(int buildIndex)
    {
        // For this to work, we need to make sure time scale is reset to 1f
        Time.timeScale = 1f;

        // Failsafe, since the buildIndex is not recognized
        // We will transition to the main menu instead
        if (!Application.CanStreamedLevelBeLoaded(buildIndex))
            StartCoroutine(TransitionToSceneRoutine(0));
        else
            StartCoroutine(TransitionToSceneRoutine(buildIndex));
    }

    /// <summary>
    /// Fades to black
    /// Stops all sounds
    /// Loads the given scene index
    /// </summary>
    /// <param name="buildIndex"></param>
    /// <returns></returns>
    IEnumerator TransitionToSceneRoutine(int buildIndex)
    {
        IsTransitioning = true;
        yield return StartCoroutine(Fader.FadeOutRoutine());

        // Make sure to clear the pool for the next scene
        AudioManager.instance.StopSFXs();
        LoadScene(buildIndex);
        IsTransitioning = false;
    }

    public void FadeIn() => StartCoroutine(FadeInRoutine());

    public IEnumerator FadeInRoutine()
    {
        IsTransitioning = true;
        yield return StartCoroutine(Fader.FadeInRoutine());
        IsTransitioning = false;
    }

    public IEnumerator FadeOutRoutine()
    {
        IsTransitioning = true;
        yield return StartCoroutine(Fader.FadeOutRoutine());
        IsTransitioning = false;
    }

    /// <summary>
    /// Aliases
    /// </summary>
    /// <param name="buildIndex"></param>
    public void LoadScene(int buildIndex) => SceneManager.LoadScene(buildIndex);
    public void LoadScene(string levelName) => SceneManager.LoadScene(levelName);
}