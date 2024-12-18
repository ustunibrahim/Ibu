using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform character; // Takip edilecek karakter (ör. "ibu")
    public float smoothSpeed = 0.8f; // Kameranın yumuşaklık oranı
    public Vector3 offset; // Kameranın başlangıçtaki konum ofseti
    public float minY = -3f; // Kameranın minimum y değeri
    public float maxY = 0;  // Kameranın maksimum y değeri


    void Start()
    {
        // Kamerayı karakterin başlangıç pozisyonuna göre ayarla
        if (character != null)
        {
            transform.position = character.position + offset;
        }
    }

    void LateUpdate()
    {
        if (character != null)
        {
            // Kameranın hedef pozisyonunu hesapla
            Vector3 targetPosition = character.position + offset;

            // Z eksenini sabitle
            targetPosition.z = transform.position.z;

            // Y eksenini sınırla
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

            // Yumuşak geçişle yeni pozisyona yaklaş
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

            // Kamerayı yeni pozisyona taşı
            transform.position = smoothedPosition;
        }
    }

}