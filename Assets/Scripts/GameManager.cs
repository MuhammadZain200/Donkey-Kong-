using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int level;
    private int lives;
    private int score;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        NewGame();
    }

    private void NewGame()
    {
        lives = 3;
        score = 0;

        LoadLevel(1);
    }

    private void LoadLevel(int index)
    {
        level = index;

        // Get the main camera in the scene
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            // Example: enable all layers (adjust if needed)
            mainCamera.cullingMask = -1;
        }

        // Load the scene after a short delay
        Invoke(nameof(LoadScene), 1f);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(level);
    }

    public void LevelComplete()
    {
        score += 1000;

        int nextLevel = level + 1;

        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            LoadLevel(nextLevel);
        }
        else
        {
            LoadLevel(1);
        }
    }

    public void LevelFail()
    {
        lives--;
        if (lives <= 0)
        {
            NewGame();
        }
        else
        {
            LoadLevel(level);
        }
    }
}
