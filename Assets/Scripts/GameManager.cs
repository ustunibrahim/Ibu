using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton referansı
    public Text scoreText;
    private int score = 0; // Başlangıç skoru

    void Awake()
    {
        // Singleton tasarımı
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (scoreText == null)
        {
            Debug.LogError("Score Text atanmadı! Lütfen Canvas içindeki TextMeshPro nesnesini 'ScoreText' alanına bağlayın.");
        }
    }

    public void AddScore(int amount)
    {
        score += amount;

        if (scoreText != null)
        {
            scoreText.text = "" + score; // Skoru güncelle
        }
        else
        {
            Debug.LogError("Score Text atanmadı!");
        }
    }
}
