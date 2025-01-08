using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarManager : MonoBehaviour
{
    public List<GameObject> stars; // Canvas'taki yıldızları tutacak liste
    public GameObject gameOverPanel; // Game Over paneli referansı
    public Sayac timer; // Timer scripti referansı
    private int currentStars;

    private const string StarsKey = "CurrentStars"; // PlayerPrefs anahtarı
    private const int MaxStars = 5; // Maksimum yıldız sayısı

    void Start()
    {
        LoadStars(); // Yıldız sayısını kayıttan yükle
        gameOverPanel.SetActive(false); // Game Over paneli kapalı başlasın

        // Timer referansını al
        if (timer == null)
        {
            timer = Sayac.Instance;
        }
    }

    public void YildizArttir()
    {
        if (currentStars < MaxStars) // Maksimum yıldız sayısını aşma
        {
            stars[currentStars].SetActive(true);
            currentStars++;
            SaveStars(); // Yıldız sayısını kaydet
        }
    }

    public void YildizAzalt()
    {
        if (currentStars > 0)
        {
            currentStars--;
            stars[currentStars].SetActive(false);
            SaveStars(); // Yıldız sayısını kaydet
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

        // Skoru kontrol et ve güncelle
        GameManager.instance.CheckHighScore();

        // Skoru sıfırlamak için GameManager'dan ResetScore metodunu çağır
        GameManager.instance.ResetScore();

        // Yıldız sayısını 5 yapalım ve kaydedelim
        currentStars = MaxStars;
        for (int i = 0; i < MaxStars; i++)
        {
            stars[i].SetActive(true); // Tüm yıldızları tekrar aktif et
        }

        SaveStars(); // Yıldız sayısını kaydet

        // Sayaç sıfırlandı
        if (timer != null)
        {
            timer.ResetTimer(); // Sayaç sıfırlandı
        }

        // Oyun sonu skor ve süre bilgisini göster
        Debug.Log("Game Over! Score: " + GameManager.instance.score + " Time: " + Sayac.Instance.GetCurrentTime());
    }

    // Yıldız sayısını kaydet
    private void SaveStars()
    {
        PlayerPrefs.SetInt(StarsKey, currentStars); // PlayerPrefs'te yıldız sayısını kaydet
        PlayerPrefs.Save();
    }

    // Yıldız sayısını yükle
    private void LoadStars()
    {
        // PlayerPrefs'ten yıldız sayısını al, varsayılan olarak 5 yıldız ile başla
        currentStars = PlayerPrefs.GetInt(StarsKey, MaxStars);

        // Yıldızları güncelle
        for (int i = 0; i < currentStars; i++)
        {
            stars[i].SetActive(true); // Yıldızları aktif et
        }
        // Kalan yıldızları gizle
        for (int i = currentStars; i < MaxStars; i++)
        {
            stars[i].SetActive(false); // Geriye kalan yıldızları gizle
        }
    }
}