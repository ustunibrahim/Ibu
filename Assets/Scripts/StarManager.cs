using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    public List<GameObject> stars; // Canvas'taki yıldızları tutacak liste
    public GameObject gameOverPanel; // Game Over paneli referansı
    private int currentStars;

    void Start()
    {
        currentStars = stars.Count; // Başlangıçtaki yıldız sayısını al
        gameOverPanel.SetActive(false); // Game Over paneli kapalı başlasın
    }

    public void YildizArttir()
    {
        if (currentStars < stars.Count) // Maksimum yıldız sayısını aşma
        {
            stars[currentStars].SetActive(true);
            currentStars++;
        }
    }

    public void YildizAzalt()
    {
        if (currentStars > 0)
        {
            currentStars--;
            stars[currentStars].SetActive(false);
        }

        if (currentStars == 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0; // Oyunu durdur
    }
}
