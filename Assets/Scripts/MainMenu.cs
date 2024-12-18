using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Scene geçişleri için
using UnityEngine.UI;  // UI elemanlarıyla etkileşim için

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel; // Ayarlar paneli referansı
    public Button settingsButton;    // Ayarlar butonu
    public Button closeSettingsButton; // Ayarlar panelini kapatma butonu
    public Toggle soundToggle; // Ses açma/kapama toggle'ı referansı
    public Image checkmarkImage; // Toggle'ın checkmark image'ını kontrol etmek için
    public Sprite soundOnIcon; // Ses açıkken kullanılacak ikon
    public Sprite soundOffIcon; // Ses kapalıyken kullanılacak ikon

    private AudioSource[] audioSources; // Tüm ses kaynaklarını tutacak array (müzik, ses efektleri vb.)

    void Start()
    {
        // Sahnedeki tüm AudioSource bileşenlerini bul ve audioSources dizisine ekle
        audioSources = FindObjectsOfType<AudioSource>();

        // Ayarlar butonuna basıldığında paneli açacak listener ekliyoruz
        settingsButton.onClick.AddListener(OnSettingsButtonClicked);

        // Ayarlar panelini kapatma butonuna listener ekliyoruz
        closeSettingsButton.onClick.AddListener(OnCloseSettingsButtonClicked);

        // Başlangıçta ayarlar panelini gizle
        settingsPanel.SetActive(false);

        // Başlangıçta, sesin durumu neyse ona göre toggle'ı ayarlayalım
        soundToggle.isOn = (AudioListener.volume == 1); // Sesin açılmış mı kapalı mı olduğuna göre toggle'ı ayarla
        soundToggle.onValueChanged.AddListener(OnSoundToggleChanged); // Toggle değeri değiştiğinde ses durumunu kontrol et

        UpdateCheckmarkIcon(); // Ses durumu ile ikonları güncelle
    }

    // Play butonuna tıklanırsa oyun sahnesine geçiş yapacak
    public void OnPlayButtonClicked()
    {
        Time.timeScale = 1; 
        StartCoroutine(LoadGameSceneAsync());
    }

    private IEnumerator LoadGameSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        // Sahne tamamen yüklendiğinde oyun başlatılabilir.
        PlatformSpawner.isGameStarted = true;
    }


    // Settings butonuna tıklanırsa ayarlar panelini açacak
    public void OnSettingsButtonClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true); // Ayarlar panelini aktif et
        }
    }

    // Ayarlar panelinde geri butonuna tıklanırsa paneli kapatacak
    public void OnCloseSettingsButtonClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false); // Ayarlar panelini gizle
        }
    }

    // Ses toggle'ı değiştiğinde ses açma/kapama işlemi yapılacak
    void OnSoundToggleChanged(bool isOn)
    {
        if (isOn)
        {
            AudioListener.volume = 1; // Ses aç
            SetAudioSourcesMute(false); // Tüm ses kaynaklarını aç
        }
        else
        {
            AudioListener.volume = 0; // Ses kapalı
            SetAudioSourcesMute(true); // Tüm ses kaynaklarını kapat
        }

        UpdateCheckmarkIcon(); // Ses durumuna göre ikonu güncelle
    }

    // Ses kaynaklarını kontrol eden metod
    void SetAudioSourcesMute(bool mute)
    {
        if (audioSources != null && audioSources.Length > 0)
        {
            foreach (var source in audioSources)
            {
                if (source != null)
                {
                    source.mute = mute; // Ses kaynağını kapat veya aç
                }
            }
        }
        else
        {
            Debug.LogWarning("audioSources dizisi boş veya null!");
        }
    }

    // Ses durumu değiştikçe checkmark ikonunu günceller
    void UpdateCheckmarkIcon()
    {
        if (checkmarkImage != null)
        {
            checkmarkImage.sprite = soundToggle.isOn ? soundOnIcon : soundOffIcon;
        }
    }
}
