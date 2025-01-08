using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Sayac : MonoBehaviour
{
    public static Sayac Instance; // Singleton örneği
    public Text timerText; // Süreyi gösterecek Text bileşeni
    private float currentTime = 0f; // Geçen süre
    private bool isTimerRunning = true; // Sayaç çalışıyor mu?

    private const string TimerKey = "CurrentTime"; // PlayerPrefs anahtarı

    void Awake()
    {
        // Singleton yapısı
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahne değişikliklerinde nesne yok olmasın
        }
        else
        {
            Destroy(gameObject); // Başka bir Timer örneği varsa yok et
        }
    }

    void Start()
    {
        LoadTimer(); // Süreyi kayıttan yükle
        FindTimerText(); // TimerText'i bul
    }

    void OnEnable()
    {
        // Sahne değişikliğinde TimerText'i yeniden bul
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Sahne yüklendiğinde çağrılır
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindTimerText(); // Yeni sahneye geçtiğinde TimerText'i bul
    }

    // TimerText'i bul
    private void FindTimerText()
    {
        // Sahnedeki TimerText'i bul
        GameObject timerTextObj = GameObject.FindGameObjectWithTag("TimerText");
        if (timerTextObj != null)
        {
            timerText = timerTextObj.GetComponent<Text>();
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime; // Süreyi güncelle
            UpdateTimerDisplay(); // Süreyi ekranda göster
        }
    }

    // Süreyi ekranda güncelle
    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60f); // Dakikaları hesapla
            int seconds = Mathf.FloorToInt(currentTime % 60f); // Saniyeleri hesapla
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Formatla ve göster
        }
    }

    // Sayaç sıfırlama
    public void ResetTimer()
    {
        currentTime = 0f; // Süreyi sıfırla
        SaveTimer(); // Süreyi kaydet
        UpdateTimerDisplay(); // Ekranı güncelle
    }

    // Sayaç durdurma
    public void StopTimer()
    {
        isTimerRunning = false; // Sayaç durduruldu
        SaveTimer(); // Süreyi kaydet
    }

    // Sayaç başlatma
    public void StartTimer()
    {
        isTimerRunning = true; // Sayaç başlatıldı
    }

    // Süreyi kaydet
    private void SaveTimer()
    {
        PlayerPrefs.SetFloat(TimerKey, currentTime); // Süreyi PlayerPrefs'e kaydet
        PlayerPrefs.Save();
    }

    // Süreyi yükle
    private void LoadTimer()
    {
        if (PlayerPrefs.HasKey(TimerKey))
        {
            currentTime = PlayerPrefs.GetFloat(TimerKey, 0f); // PlayerPrefs'ten süreyi yükle, yoksa 0'dan başla
        }
        else
        {
            currentTime = 0f; // Eğer kayıtlı süre yoksa sıfırdan başla
        }
    }

    // Geçen süreyi döndür
    public float GetCurrentTime()
    {
        return currentTime;
    }

    void OnApplicationQuit()
    {
        SaveTimer(); // Oyun kapatıldığında süreyi kaydet
    }
}