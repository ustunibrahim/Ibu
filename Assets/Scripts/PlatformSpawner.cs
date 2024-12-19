using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlatformSpawner : MonoBehaviour
{
    public static bool isGameStarted = false;
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

    private void Awake()
    {
        if (!isGameStarted)
        {
            Start();
            isGameStarted = true;
        }
    }

    void Start()
    {
        nextSpawnPosition = startPlatform.position + new Vector3(platformWidth, 0, 0);
        nextCloudPosition = cloudPrefab.transform.position;

        playerRb = player.GetComponent<Rigidbody2D>();

        currentBackground = Instantiate(backgroundPrefab);
        Vector3 backgroundPos = currentBackground.transform.position;
        backgroundPos.y = startPlatform.position.y - platformHeight / 2;
        currentBackground.transform.position = backgroundPos;
        AdjustBackgroundToScreen();

        // Arka planın tam ekran olması için RectTransform ayarları
        RectTransform rectTransform = currentBackground.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero;   // Sol alt köşe
            rectTransform.anchorMax = Vector2.one;    // Sağ üst köşe
            rectTransform.offsetMin = Vector2.zero;   // İçeriden uzaklık (0)
            rectTransform.offsetMax = Vector2.zero;   // Dışarıdan uzaklık (0)
        }

        starManager = FindObjectOfType<StarManager>();
    }

    void AdjustBackgroundToScreen()
    {
        if (currentBackground == null) return;

        SpriteRenderer spriteRenderer = currentBackground.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            float worldScreenHeight = Camera.main.orthographicSize * 2f;
            float worldScreenWidth = worldScreenHeight * Camera.main.aspect;

            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

            float scaleX = worldScreenWidth / spriteSize.x;
            float scaleY = worldScreenHeight / spriteSize.y;

            float finalScale = Mathf.Max(scaleX, scaleY);

            currentBackground.transform.localScale = new Vector3(finalScale, finalScale, 1);
        }
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

        GameObject cloudGroup = new GameObject("CloudGroup");

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

        GameObject balloonGroup = new GameObject("BalloonGroup");
        balloonGroup.transform.SetParent(cloudGroup.transform); // CloudGroup'a ekle

        for (int i = 0; i < answers.Count; i++)
        {
            GameObject balloon = Instantiate(mathBalloonPrefab, balloonPosition, Quaternion.identity);
            balloon.transform.SetParent(balloonGroup.transform);  // Baloncukları grup altında oluştur
            AttachAnswerToBalloon(balloon, answers[i]);
            balloonPosition.x += 4f;
        }

        BalloonGroup balloonGroupScript = balloonGroup.AddComponent<BalloonGroup>(); // BalloonGroup scriptini ekle
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
                if (a < b) { int temp = a; a = b; b = temp; }
                currentMathQuestion = $"{a} - {b}";
                correctAnswer = a - b;
                break;
            case 2: // Çarpma
                currentMathQuestion = $"{a} x {b}";
                correctAnswer = a * b;
                break;
            case 3: // Bölme
                b = Mathf.Max(1, b); // b'nin 0 olmasını engellemek için
                a = b * Random.Range(1, 10);
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
            GameObject textGO = new GameObject("MathQuestionText");
            textGO.transform.SetParent(cloud.transform, false);
            textGO.transform.localPosition = Vector3.zero;

            Canvas canvas = textGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = textGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.referencePixelsPerUnit = 100;

            text = textGO.AddComponent<TextMeshProUGUI>();
            text.fontSize = 0.8f;
            text.alignment = TextAlignmentOptions.Center;
        }

        text.text = currentMathQuestion;

        if (ColorUtility.TryParseHtmlString("#00b2ee", out Color newColor))
        {
            text.color = newColor;
        }
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
        text.fontSize = 6;
        text.alignment = TextAlignmentOptions.Center;

        if (ColorUtility.TryParseHtmlString("#00b2ee", out Color newColor))
        {
            text.color = newColor;
        }

        MathBalloon balloonScript = balloon.AddComponent<MathBalloon>();
        balloonScript.dogruMu = (answer == correctAnswer);

        balloonScript.dogruSound = Resources.Load<AudioClip>("Sounds/dogruCevap");
        balloonScript.yanlisSound = Resources.Load<AudioClip>("Sounds/yanlisCevap");
    }

    List<int> GenerateAnswerOptions()
    {
        List<int> answers = new List<int> { correctAnswer };
        while (answers.Count < 3)
        {
            int wrongAnswer = Random.Range(correctAnswer - 10, correctAnswer + 10);
            if (wrongAnswer >= 0 && !answers.Contains(wrongAnswer))
            {
                answers.Add(wrongAnswer);
            }
        }
        return ShuffleList(answers);
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
