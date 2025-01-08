using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Text highScoreText; // Skor tablosu için Text elemanı
    public Text scoreText;
    public GameObject pauseMenuPanel;
    private int score = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScore(); // Skoru yükle
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // Scene yüklendiğinde tetiklenecek
        FindUIObjects();
        UpdateScoreText();
        UpdateHighScoreText(); // Skor tablosunu güncelle
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindUIObjects();  // PauseMenuPanel'i her sahnede yeniden bul
        UpdateScoreText(); // Skoru güncelle
        UpdateHighScoreText(); // Skor tablosunu güncelle
    }
    private void FindUIObjects()
    {
        if (scoreText == null)
        {
            scoreText = GameObject.FindWithTag("ScoreText")?.GetComponent<Text>();
        }
        if (pauseMenuPanel == null)
        {
            // PauseMenuPanel'i her sahnede bul
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                pauseMenuPanel = canvas.transform.Find("PausePanel")?.gameObject;
                if (pauseMenuPanel != null)
                {
                    pauseMenuPanel.SetActive(false); // Başlangıçta gizle
                }
            }
        }
        if (highScoreText == null)
        {
            highScoreText = GameObject.FindWithTag("HighScoreText")?.GetComponent<Text>();
        }
    }
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
        SaveScore(); // Skoru kaydet
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    private void SaveScore()
    {
        PlayerPrefs.SetInt("PlayerScore", score); // Skoru kaydet
        PlayerPrefs.Save();
    }

    private void LoadScore()
    {
        score = PlayerPrefs.GetInt("PlayerScore", 0); // Skoru yükle, yoksa 0
    }

    public void ResetScore()
    {
        score = 0;
        SaveScore(); // Skoru sıfırla ve kaydet
        UpdateScoreText();
    }
    public void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
        }
    }
    public void CheckHighScore()
    {
        int currentScore = score;
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
            UpdateHighScoreText();
        }
    }
}
