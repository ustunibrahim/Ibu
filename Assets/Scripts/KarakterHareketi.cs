using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class KarakterHareketi : MonoBehaviour
{
    public float hiz = 5f;  // Karakterin normal hızı
    public float ziplamaGucu = 5f; // Zıplama gücü
    private bool yerleTemas = false; // Karakterin yerle temas halinde olup olmadığını kontrol eder
    private bool doubleJumpYapilabilir = false; // Double jump kontrolü
    private float orijinalHiz; // Karakterin normal hızını saklar
    private bool hizDegisiyor = false; // Hızın geçici olarak değiştiğini kontrol eder
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D bileşenini alıyoruz
        orijinalHiz = hiz; // Başlangıç hızını kaydet
    }

    private void Update()
    {
        // Karakterin sağa doğru hareket etmesini sağlıyoruz
        if (!hizDegisiyor)
        {
            transform.Translate(Vector2.right * hiz * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.right * hiz * Time.deltaTime);
        }

        // Ekrana tıklama kontrolü
        if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && !IsClickingOnUI())
        {
            if (yerleTemas) // Yerdeyken birinci zıplama
            {
                Jump();
            }
            else if (doubleJumpYapilabilir) // Havada iken ikinci zıplama
            {
                DoubleJump();
            }
        }
    }

    // Zıplama fonksiyonu
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, ziplamaGucu);
        yerleTemas = false;
        doubleJumpYapilabilir = true;
    }

    // Double jump fonksiyonu
    private void DoubleJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, ziplamaGucu);
        doubleJumpYapilabilir = false;
    }

    // Yere temas kontrolü
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            yerleTemas = true;
            doubleJumpYapilabilir = false;
        }
    }

  
   

    // Hızın geçici olarak değiştirilmesini sağlayan fonksiyon
    public void HiziGeciciOlarakDegistir(float yeniHiz, float sure)
    {
        if (hizDegisiyor) return; // Eğer hız zaten değiştiyse çık
        StartCoroutine(HiziGeciciDegistirCoroutine(yeniHiz, sure));
    }

    private IEnumerator HiziGeciciDegistirCoroutine(float yeniHiz, float sure)
    {
        hizDegisiyor = true;
        float eskiHiz = hiz;
        hiz = yeniHiz;

        yield return new WaitForSeconds(sure);

        hiz = eskiHiz;
        hizDegisiyor = false;
    }

    // UI tıklamalarını kontrol eden fonksiyon
    private bool IsClickingOnUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}