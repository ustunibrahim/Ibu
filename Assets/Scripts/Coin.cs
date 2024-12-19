using UnityEngine;

public class Coin : MonoBehaviour
{
    public int scoreValue = 1; // Coin'in katkı değeri
    public AudioClip coinSound; // Coin sesi için ses dosyası
    private AudioSource audioSource; // Ses çalmak için AudioSource
    private bool isCollected = false; // Coin'in toplandığını kontrol etmek için

    void Start()
    {
        // GameObject'e bir AudioSource ekle
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false; // Başlangıçta ses çalmasın
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected) // Karakter coin'e dokundu mu?
        {
            isCollected = true; // Coin bir kez toplanabilir
            PlayCoinSound(); // Coin sesini çal
            GameManager.instance.AddScore(scoreValue); // Skoru artır

            // Coin'i görünmez yaparak hemen kayboluyormuş gibi göster
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;

            // Sesin uzunluğu kadar bir süre sonra objeyi yok et
            Destroy(gameObject, coinSound.length);
        }
    }

    void PlayCoinSound()
    {
        if (coinSound != null) // Ses dosyası atanmış mı?
        {
            audioSource.PlayOneShot(coinSound); // Ses çal
        }
    }
}
