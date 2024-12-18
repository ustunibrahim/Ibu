using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel; // Menü paneli
    private bool isPaused = false;   // Oyun duraklatıldı mı?

    // Pause butonuna tıklanınca çağrılır
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0; // Oyunu durdur
            pauseMenuPanel.SetActive(true); // Menüyü göster
        }
        else
        {
            Time.timeScale = 1; // Oyunu devam ettir
            pauseMenuPanel.SetActive(false); // Menüyü gizle
        }
    }

    // Resume butonuna tıklanınca çağrılır
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1; // Oyunu devam ettir
        pauseMenuPanel.SetActive(false); // Menüyü gizle
    }

    // Quit butonuna tıklanınca çağrılır (opsiyonel)
    public void QuitGame()
    {
        Time.timeScale = 1; // Oyunu duraklatmadan kapat
        Application.Quit(); // Oyundan çık (Editor'de çalışmaz)
    }
}
