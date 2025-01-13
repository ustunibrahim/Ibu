using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class KarakterHareketi : MonoBehaviour
{
    public float hiz = 4f;
    public float ziplamaGucu = 6f;
    public float hizArtisCarpani = 1.5f; // Hız artış çarpanı eklendi
    public float hizArtisSuresi = 4f; // Hız artış süresi eklendi
    public AudioClip carrotSound; // Carrot ses dosyası
    public AudioClip jumpSound; // Zıplama ses dosyası

    private bool yerleTemas = false;
    private float orijinalHiz;
    private bool hizDegisiyor = false;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        orijinalHiz = hiz;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (!hizDegisiyor)
        {
            transform.Translate(Vector2.right * hiz * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.right * hiz * Time.deltaTime);
        }

        if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && !IsClickingOnUI())
        {
            if (yerleTemas)
            {
                Jump();
            }
        }
    }

    private void Jump()
    {
        if (Time.timeScale > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, ziplamaGucu);
            yerleTemas = false;

            // Zıplama sesini çal
            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            yerleTemas = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D carpisma)
    {
        if (carpisma.gameObject.CompareTag("Carrot"))
        {
            HiziGeciciOlarakDegistir(hiz * hizArtisCarpani, hizArtisSuresi);
            if (carrotSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(carrotSound);
            }
            Destroy(carpisma.gameObject);
        }
    }

    public void HiziGeciciOlarakDegistir(float yeniHiz, float sure)
    {
        if (hizDegisiyor) return;
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

    private bool IsClickingOnUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}