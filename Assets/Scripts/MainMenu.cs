using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public Button settingsButton;
    public Button closeSettingsButton;
    public Toggle soundToggle;
    public Image checkmarkImage;
    public Sprite soundOnIcon;
    public Sprite soundOffIcon;

    private AudioSource[] audioSources;

    void Start()
    {
        audioSources = FindObjectsOfType<AudioSource>();

        settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        closeSettingsButton.onClick.AddListener(OnCloseSettingsButtonClicked);

        settingsPanel.SetActive(false);

        soundToggle.isOn = (AudioListener.volume == 1);
        soundToggle.onValueChanged.AddListener(OnSoundToggleChanged);

        UpdateCheckmarkIcon();
    }

    public void OnPlayButtonClicked()
    {
        Time.timeScale = 1; // Zaman akışını sıfırla
        SceneManager.LoadScene("GameScene");
    }

    public void OnSettingsButtonClicked()
    {
        settingsPanel.SetActive(true);
    }

    public void OnCloseSettingsButtonClicked()
    {
        settingsPanel.SetActive(false);
    }

    private void OnSoundToggleChanged(bool isOn)
    {
        AudioListener.volume = isOn ? 1 : 0;
        SetAudioSourcesMute(!isOn);
        UpdateCheckmarkIcon();
    }

    private void SetAudioSourcesMute(bool mute)
    {
        foreach (var source in audioSources)
        {
            if (source != null)
            {
                source.mute = mute;
            }
        }
    }

    private void UpdateCheckmarkIcon()
    {
        if (checkmarkImage != null)
        {
            checkmarkImage.sprite = soundToggle.isOn ? soundOnIcon : soundOffIcon;
        }
    }
}
