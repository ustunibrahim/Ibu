using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0;
            GameManager.instance.pauseMenuPanel?.SetActive(true);
            
        }
        else
        {
            Time.timeScale = 1;
            GameManager.instance.pauseMenuPanel?.SetActive(false);
            
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;  // Zamanı geri başlat
        if (GameManager.instance.pauseMenuPanel != null)
        {
            GameManager.instance.pauseMenuPanel.SetActive(false); // Pause menüsünü gizle
        }
       
    }

    public void MainMenu()
    {
        Time.timeScale = 1;  // Zamanı sıfırla
        SceneManager.LoadScene("Menu"); // Ana menüye git
    }
    public void Return()
    {
        Time.timeScale = 1;  // Zamanı sıfırla
        SceneManager.LoadScene("GameScene"); // Ana menüye git
    }



    public void QuitGame()
    {
        Time.timeScale = 1; // Zamanı sıfırla
        Application.Quit();
        Debug.Log("Oyun kapatıldı.");
    }
}
