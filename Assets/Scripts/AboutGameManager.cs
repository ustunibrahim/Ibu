using UnityEngine;
using UnityEngine.UI;
public class AboutGameManager : MonoBehaviour
{
    public GameObject aboutGamePanel; // AboutGamePanel GameObject'i
    public Button aboutGameButton; // AboutGameButton Butonu
    public Button closeAboutGameButton; // Kapat Butonu

    public void OnAboutGameButtonClicked()
    {
        aboutGamePanel.SetActive(true);
    }
    public void OnCloseAboutGameButtonClicked()
    {
        aboutGamePanel.SetActive(false);
    }
}