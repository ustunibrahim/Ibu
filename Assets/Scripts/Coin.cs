using UnityEngine;

public class Coin : MonoBehaviour
{
    public int scoreValue = 1; // Coin'in katkı değeri

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Karakter coin'e dokundu mu?
        {
            GameManager.instance.AddScore(scoreValue); // Skoru artır
            Destroy(gameObject); // Coin'i yok et
        }
    }
}
