using UnityEngine;

public class Elixir : MonoBehaviour
{
    public AudioClip plusOneHealthSound; // +1 can ses dosyası
    public AudioClip poisonSound;       // Zehir ses dosyası
    public AudioClip plusTwoHealthSound; // +2 can ses dosyası

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Eğer çarpışan obje "Player" tag'ine sahipse
        if (collision.CompareTag("Player"))
        {
            StarManager starManager = FindObjectOfType<StarManager>();

            // Çarpışılan objenin tag'ini kontrol et
            if (gameObject.CompareTag("+1 can"))
            {
                starManager.YildizArttir();
                SoundManager.Instance.PlaySound(plusOneHealthSound); // +1 can sesini çal
            }
            else if (gameObject.CompareTag("zehir"))
            {
                starManager.YildizAzalt();
            }
            else if (gameObject.CompareTag("+2 can"))
            {
                starManager.DoubleYildizArttir();
                SoundManager.Instance.PlaySound(plusTwoHealthSound); // +2 can sesini çal
            }

            Destroy(gameObject); // Prefab'ı yok et
        }
    }
}