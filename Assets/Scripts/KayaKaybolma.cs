using UnityEngine;
using DG.Tweening;  // DOTween namespace


public class KayaKaybolma : MonoBehaviour
{
    private bool kayboluyorMu = false;
    public float geciciHiz = 2f;  // Hız azaltma oranı
    public float yavaslamaSuresi = 3f;  // Yavaşlama süresi

    public KarakterHareketi karakterHareketi;

    void Start()
    {
        if (karakterHareketi == null)
        {
            karakterHareketi = FindObjectOfType<KarakterHareketi>();
        }
    }

    void OnTriggerEnter2D(Collider2D diger)
    {
        if (diger.CompareTag("Player") && !kayboluyorMu)
        {
            kayboluyorMu = true;

            if (karakterHareketi != null)
            {
                karakterHareketi.HiziGeciciOlarakDegistir(geciciHiz, yavaslamaSuresi);
            }

            transform.DOScale(Vector3.zero, 0.5f).OnKill(() => Destroy(gameObject));
        }
    }
}
