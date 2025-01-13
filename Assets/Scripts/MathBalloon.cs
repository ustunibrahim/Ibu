using UnityEngine;
using System.Collections;

public class MathBalloon : MonoBehaviour
{
    public bool dogruMu; // Balonun doğru cevabı mı taşıdığı bilgisini tutar
    public AudioClip dogruSound; // Doğru cevap sesi
    public AudioClip yanlisSound;   // Yanlış cevap sesi
    private AudioSource audioSource;
    private bool alreadyInteracted = false;  // İlk etkileşimi takip etmek için değişken
    private BalloonGroup parentGroup;

    private void Start()
    {
        // Ses kaynağını ayarla
        audioSource = gameObject.AddComponent<AudioSource>();

        // Balonun ait olduğu grup (parentGroup) bilgisini al
        parentGroup = transform.parent.GetComponentInParent<BalloonGroup>(); // ParentGroup'a referans al
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !alreadyInteracted)
        {
            alreadyInteracted = true;  // İlk etkileşimi işaretle
            StarManager starManager = FindObjectOfType<StarManager>();

            if (dogruMu)
            {
               // starManager.YildizArttir(); // Doğru cevap için yıldız artır
                GameManager.instance.TrueBallon(); // Skora 100 ekler
                StartCoroutine(PlaySoundAndDestroy(dogruSound));   // Doğru cevap sesi çal ve baloncukları yok et
            }
            else
            {
                starManager.YildizAzalt(); // Yanlış cevap için yıldız azalt
                StartCoroutine(PlaySoundAndDestroy(yanlisSound));    // Yanlış cevap sesi çal ve baloncukları yok et
            }

            parentGroup.DisableOtherBalloons(this); // Diğer baloncukları devre dışı bırak
        }
    }

    private IEnumerator PlaySoundAndDestroy(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);  // Sesi çalmaya başla
        }

        // Sesin çalınmaya başlamasından sonra maksimum süreyi bekle
        yield return new WaitForSeconds(Mathf.Min(clip.length, 1f));

        // Ses bitince baloncuğun bulunduğu grubun tamamını yok et
        Destroy(transform.parent.gameObject); // Sesin çalması için kısa bir gecikme ekle
    }
}