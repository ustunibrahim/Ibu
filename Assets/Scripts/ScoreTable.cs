using UnityEngine;
using UnityEngine.UI;

public class ScoreTable : MonoBehaviour
{
    public GameObject scorePanel;
    public Button scoreButton;
    public Button closeScore;
    public Text highScoreText;

    void Start()
    {
        UpdateHighScoreText();
    }

    public void OnScoreButtonClicked()
    {
        scorePanel.SetActive(true);
    }

    public void OnCloseScoreClicked()
    {
        scorePanel.SetActive(false);
    }

    public void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            float bestTime = PlayerPrefs.GetFloat("BestTime", 0f);
            int minutes = Mathf.FloorToInt(bestTime / 60f);
            int seconds = Mathf.FloorToInt(bestTime % 60f);
            highScoreText.text = "En İyi Derece: " + highScore.ToString() + " (Süre: " + string.Format("{0:00}:{1:00}", minutes, seconds) + ")";
        }
    }
}