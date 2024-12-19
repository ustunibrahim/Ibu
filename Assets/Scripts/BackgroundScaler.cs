using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{
    void Start()
    {
        ScaleToScreenSize();
    }

    void ScaleToScreenSize()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Kameranın boyutlarını al
            float screenHeight = Camera.main.orthographicSize * 2f;
            float screenWidth = screenHeight * Camera.main.aspect;

            // Sprite boyutlarını al
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

            // Ölçekleme faktörlerini hesapla
            float scaleX = screenWidth / spriteSize.x;
            float scaleY = screenHeight / spriteSize.y;

            // Maksimum ölçek faktörünü seç ve uygula
            float finalScale = Mathf.Max(scaleX, scaleY);
            transform.localScale = new Vector3(finalScale, finalScale, 1);
        }
    }
}
