using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform character; // Takip edilecek karakter
    public float smoothSpeed = 0.125f; // Kameranın yumuşaklık oranı
    public Vector3 offset; // Kameranın başlangıçtaki konum ofseti
    public float minY = -3f; // Kameranın minimum y değeri
    public float maxY = 0f;  // Kameranın maksimum y değeri

    public SpriteRenderer backgroundRenderer; // Arka plan SpriteRenderer'ı
    public Vector3 backgroundOffset; // Arka planın ofset değeri

    private Vector3 velocity = Vector3.zero; // SmoothDamp için hız vektörü

    void Start()
    {
        // Kamerayı karakterin başlangıç pozisyonuna göre ayarla
        if (character != null)
        {
            transform.position = character.position + offset;
        }

        // Arka planı ekran boyutuna göre ölçeklendir
        AdjustBackgroundSize();
    }

    void LateUpdate()
    {
        if (character != null)
        {
            // Kameranın hedef pozisyonunu hesapla
            Vector3 targetPosition = character.position + offset;

            // Z eksenini sabitle (kameranın derinliğini koru)
            targetPosition.z = transform.position.z;

            // Y eksenini sınırla (kameranın belirli bir aralıkta kalmasını sağla)
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

            // Yumuşak geçişle yeni pozisyona yaklaş (SmoothDamp kullanarak)
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);

            // Arka planın pozisyonunu kameranın pozisyonu ile senkronize et
            UpdateBackgroundPosition();
        }
    }

    void AdjustBackgroundSize()
    {
        if (backgroundRenderer == null) return;

        // Ekran boyutlarını hesapla
        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * Camera.main.aspect;

        // Sprite'ın boyutunu al
        Vector2 spriteSize = backgroundRenderer.sprite.bounds.size;

        // Ölçeklendirme faktörlerini hesapla
        float scaleX = worldScreenWidth / spriteSize.x;
        float scaleY = worldScreenHeight / spriteSize.y;

        // Sprite'ı ekran boyutuna göre ölçeklendir
        backgroundRenderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    void UpdateBackgroundPosition()
    {
        if (backgroundRenderer != null)
        {
            // Arka planın pozisyonunu kameranın pozisyonu ile senkronize et
            Vector3 backgroundPosition = transform.position + backgroundOffset;
            backgroundPosition.z = backgroundRenderer.transform.position.z; // Z eksenini sabit tut
            backgroundRenderer.transform.position = backgroundPosition;
        }
    }
}