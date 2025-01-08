using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Text scoreText;
    public GameObject pauseMenuPanel;
    public int score = 0;

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
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindUIObjects();  // PauseMenuPanel'i her sahnede yeniden bul
        UpdateScoreText(); // Skoru güncelle
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

    public void CheckHighScore()
    {
        float currentScoreWithTime = CalculateScoreWithTime(score, Sayac.Instance.GetCurrentTime());
        float highScoreWithTime = PlayerPrefs.GetFloat("HighScoreWithTime", 0f);

        if (currentScoreWithTime > highScoreWithTime)
        {
            PlayerPrefs.SetFloat("HighScoreWithTime", currentScoreWithTime);
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.SetFloat("BestTime", Sayac.Instance.GetCurrentTime());
            PlayerPrefs.Save();
        }
    }

    public float CalculateScoreWithTime(int score, float time)
    {
        // Skoru süreye bölerek puanlama yapalım
        // Süre arttıkça puan azalacak, böylece daha hızlı bitirenler daha yüksek puan alacak
        return score / time;
    }
}