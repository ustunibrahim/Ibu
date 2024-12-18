using UnityEngine;

public class MathBalloon : MonoBehaviour
{
    public bool dogruMu; // Balonun doğru cevabı mı taşıdığı bilgisini tutar

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StarManager starManager = FindObjectOfType<StarManager>();
            if (dogruMu)
            {
                starManager.YildizArttir(); // Doğru cevap için yıldız artır
            }
            else
            {
                starManager.YildizAzalt(); // Yanlış cevap için yıldız azalt
            }

            // Baloncuğun bulunduğu grubun tamamını yok et
            Destroy(transform.parent.gameObject);
        }
    }
}
