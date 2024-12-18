using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;


public class PlatformSpawner : MonoBehaviour
{
    public GameObject platformPrefab;
    public GameObject rockPrefab;
    public GameObject backgroundPrefab;
    public GameObject coinPrefab;
    public GameObject cloudPrefab;
    public GameObject mathBalloonPrefab;

    public Transform startPlatform;
    public Transform player;
    public Transform manualRock;
    public Transform manualCoin;

    public float spawnInterval = 2f;
    public float platformWidth = 10f;
    public float rockSpawnChance = 0.2f;
    public float coinSpawnChance = 0.3f;
    public float cloudSpawnInterval = 3f;
    public float platformHeight = 1f;
    public float speedFactor = 0.5f;

    private float nextSpawnTime;
    private float nextCloudSpawnTime;
    private Vector3 nextCloudPosition;
    private Vector3 nextSpawnPosition;
    private GameObject currentBackground;
    private Rigidbody2D playerRb;
    private StarManager starManager;

    private string currentMathQuestion;
    private int correctAnswer;
   


    void Start()
    {
        nextSpawnPosition = startPlatform.position + new Vector3(platformWidth, 0, 0);
        nextCloudPosition = cloudPrefab.transform.position;

        playerRb = player.GetComponent<Rigidbody2D>();

        currentBackground = Instantiate(backgroundPrefab);
        Vector3 backgroundPos = currentBackground.transform.position;
        backgroundPos.y = startPlatform.position.y - platformHeight / 2;
        currentBackground.transform.position = backgroundPos;

        starManager = FindObjectOfType<StarManager>();
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnPlatform();
            nextSpawnTime = Time.time + spawnInterval;
        }

        if (Time.time >= nextCloudSpawnTime)
        {
            SpawnCloud();
            nextCloudSpawnTime = Time.time + cloudSpawnInterval;
        }

        UpdateBackgroundPosition();
        AdjustSpawnIntervalByPlayerSpeed();
    }

    void SpawnPlatform()
    {
        Instantiate(platformPrefab, nextSpawnPosition, Quaternion.identity);
        nextSpawnPosition += new Vector3(platformWidth, 0, 0);

        if (Random.value < rockSpawnChance)
        {
            SpawnRock();
        }

        if (Random.value < coinSpawnChance)
        {
            SpawnCoin();
        }
    }

    void SpawnRock()
    {
        float rockXPosition = Random.Range(-platformWidth / 2, platformWidth / 2);
        Vector3 rockPosition = new Vector3(nextSpawnPosition.x + rockXPosition, manualRock.position.y, nextSpawnPosition.z);
        Instantiate(rockPrefab, rockPosition, Quaternion.identity);
    }

    void SpawnCoin()
    {
        float coinXPosition = Random.Range(-platformWidth / 2, platformWidth / 2);
        Vector3 coinPosition = new Vector3(nextSpawnPosition.x + coinXPosition, manualCoin.position.y, nextSpawnPosition.z);
        Instantiate(coinPrefab, coinPosition, Quaternion.identity);
    }
    void SpawnCloud()
    {
        if (cloudPrefab == null) return;

        nextCloudPosition.x += 60f;
        Vector3 cloudPosition = new Vector3(nextCloudPosition.x, cloudPrefab.transform.position.y, 0);

        // Bulut ve baloncukları gruplayacak bir parent oluşturuyoruz
        GameObject cloudGroup = new GameObject("CloudGroup");

        // Bulut oluştur ve parent olarak ata
        GameObject cloud = Instantiate(cloudPrefab, cloudPosition, Quaternion.identity);
        cloud.transform.SetParent(cloudGroup.transform);

        GenerateMathQuestion();
        AttachMathQuestionToCloud(cloud);

        SpawnBalloonCluster(cloudPosition, cloudGroup); // Baloncukları bulut grubuna ekle
    }
    void SpawnBalloonCluster(Vector3 cloudPosition, GameObject cloudGroup)
    {
        if (mathBalloonPrefab == null) return;

        float balloonY = cloudPosition.y - 1.7f;
        Vector3 balloonPosition = new Vector3(cloudPosition.x + 10f, balloonY, 0);

        List<int> answers = GenerateAnswerOptions();
        for (int i = 0; i < answers.Count; i++)
        {
            GameObject balloon = Instantiate(mathBalloonPrefab, balloonPosition, Quaternion.identity);
            balloon.transform.SetParent(cloudGroup.transform); // Baloncukları gruba ekle
            AttachAnswerToBalloon(balloon, answers[i]);
            balloonPosition.x += 4f;
        }
    }

    void GenerateMathQuestion()
    {
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        int operation = Random.Range(0, 4);

        switch (operation)
        {
            case 0: // Toplama
                currentMathQuestion = $"{a} + {b}";
                correctAnswer = a + b;
                break;
            case 1: // Çıkarma
                    // Çıkarmada negatif sonuçları engellemek için b'yi a'dan büyük yapmamız gerekebilir
                if (a < b)
                {
                    int temp = a;
                    a = b;
                    b = temp;
                }
                currentMathQuestion = $"{a} - {b}";
                correctAnswer = a - b;
                break;
            case 2: // Çarpma
                currentMathQuestion = $"{a} x {b}";
                correctAnswer = a * b;
                break;
            case 3: // Bölme
                    // Bölme işlemi için a, b ile tam bölünebilir olmalı
                b = Mathf.Max(1, b); // b'nin 0 olmasını engellemek için
                a = b * Random.Range(1, 10); // b ile tam bölünecek a değeri üretelim
                currentMathQuestion = $"{a} ÷ {b}";
                correctAnswer = a / b;
                break;
        }
    }


    void AttachMathQuestionToCloud(GameObject cloud)
    {
        TextMeshProUGUI text = cloud.GetComponentInChildren<TextMeshProUGUI>();
        if (text == null)
        {
            // Eğer TextMeshProUGUI bileşeni yoksa, onu oluşturuyoruz
            GameObject textGO = new GameObject("MathQuestionText");
            textGO.transform.SetParent(cloud.transform, false);
            textGO.transform.localPosition = Vector3.zero;

            // Canvas ekleyelim (bu, TextMeshProUGUI'nin görünebilmesi için gerekli)
            Canvas canvas = textGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace; // WorldSpace, 3D sahnede görünmesini sağlar
            canvas.sortingOrder = 10; // Diğer öğelerin önünde görünmesini sağlar

            // Ayrıca, Canvas'ı render etmesi için bir "CanvasScaler" bileşeni eklemeyi unutmayın.
            CanvasScaler scaler = textGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.referencePixelsPerUnit = 100;

            text = textGO.AddComponent<TextMeshProUGUI>();
            text.fontSize = 1; // Font büyüklüğü
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.blue;
        }

        text.text = currentMathQuestion;
    }
    void SpawnBalloonCluster(Vector3 cloudPosition)
    {
        if (mathBalloonPrefab == null)
        {
            return;
        }

        float balloonY = cloudPosition.y - 1.7f;
        Vector3 balloonPosition = new Vector3(cloudPosition.x + 10f, balloonY, 0);

        List<int> answers = GenerateAnswerOptions();
        for (int i = 0; i < answers.Count; i++)
        {
            GameObject balloon = Instantiate(mathBalloonPrefab, balloonPosition, Quaternion.identity);
            AttachAnswerToBalloon(balloon, answers[i]);
            balloonPosition.x += 4f;
        }
    }

    List<int> GenerateAnswerOptions()
    {
        List<int> answers = new List<int> { correctAnswer };
        while (answers.Count < 3)
        {
            // Yanlış cevapların negatif olmaması için kontrol ekliyoruz
            int wrongAnswer = Random.Range(correctAnswer - 10, correctAnswer + 10);
            if (wrongAnswer >= 0 && !answers.Contains(wrongAnswer)) // Yanlış cevap negatif olmasın
            {
                answers.Add(wrongAnswer);
            }
        }
        return ShuffleList(answers);
    }


    void AttachAnswerToBalloon(GameObject balloon, int answer)
    {
        TextMeshProUGUI text = balloon.GetComponentInChildren<TextMeshProUGUI>();
        if (text == null)
        {
            GameObject textGO = new GameObject("AnswerText");
            textGO.transform.SetParent(balloon.transform, false);
            textGO.transform.localPosition = Vector3.zero;

            Canvas canvas = textGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = textGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.referencePixelsPerUnit = 50;

            text = textGO.AddComponent<TextMeshProUGUI>();
        }

        text.text = answer.ToString();
        text.fontSize = 8;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.blue;

        // Doğru cevabı belirle
        MathBalloon balloonScript = balloon.AddComponent<MathBalloon>();
        balloonScript.dogruMu = (answer == correctAnswer);
    }




    List<T> ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    void UpdateBackgroundPosition()
    {
        if (currentBackground != null && player != null)
        {
            Vector3 backgroundPos = currentBackground.transform.position;
            backgroundPos.x = player.position.x;
            currentBackground.transform.position = backgroundPos;
        }
    }

    void AdjustSpawnIntervalByPlayerSpeed()
    {
        if (playerRb != null)
        {
            float playerSpeed = Mathf.Abs(playerRb.velocity.x);
            spawnInterval = Mathf.Max(0.5f, 2f - playerSpeed * speedFactor);
        }
    }

}