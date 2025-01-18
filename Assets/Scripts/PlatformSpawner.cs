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
    public GameObject CarrotPrefab; // Carrot prefab'ı eklendi
    public GameObject plusOneHealthPrefab; // +1 can prefab'ı eklendi
    public GameObject poisonPrefab; // Zehir prefab'ı eklendi
    public GameObject plusTwoHealthPrefab; // +2 can prefab'ı eklendi

    public Transform startPlatform;
    public Transform player;
    public Transform manualRock;
    public Transform manualCoin;

    public float spawnInterval = 1.5f; // Platform spawn aralığı
    public float platformWidth = 10f;
    public float rockSpawnChance = 0.5f; // Kaya spawn olasılığı
    public float coinSpawnChance = 0.6f; // Coin spawn olasılığı
    public float CarrotSpawnChance = 0.1f; // Carrot spawn şansı
    public float plusOneHealthSpawnChance = 0.07f; // +1 can spawn şansı
    public float poisonSpawnChance = 0.07f; // Zehir spawn şansı
    public float plusTwoHealthSpawnChance = 0.01f; // +2 can spawn şansı
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

    private List<float> occupiedXPositions = new List<float>(); // Tüm prefab'lar için işgal edilmiş x konumları

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

        // Her prefab için ayrı bir x pozisyonu hesapla
        Vector3 rockPosition = GetAvailablePosition(1f); // En az 1 birim boşluk
        if (rockPosition != Vector3.zero && Random.value < rockSpawnChance)
        {
            SpawnRock(rockPosition);
        }

        Vector3 coinPosition = GetAvailablePosition(1f); // En az 1 birim boşluk
        if (coinPosition != Vector3.zero && Random.value < coinSpawnChance)
        {
            SpawnCoin(coinPosition);
        }

        Vector3 CarrotPosition = GetAvailablePosition(1f); // En az 1 birim boşluk
        if (CarrotPosition != Vector3.zero && Random.value < CarrotSpawnChance)
        {
            SpawnCarrot(CarrotPosition);
        }

        Vector3 plusOneHealthPosition = GetAvailablePosition(1f); // En az 1 birim boşluk
        if (plusOneHealthPosition != Vector3.zero && Random.value < plusOneHealthSpawnChance)
        {
            SpawnPlusOneHealth(plusOneHealthPosition);
        }

        Vector3 poisonPosition = GetAvailablePosition(1f); // En az 1 birim boşluk
        if (poisonPosition != Vector3.zero && Random.value < poisonSpawnChance)
        {
            SpawnPoison(poisonPosition);
        }

        Vector3 plusTwoHealthPosition = GetAvailablePosition(1f); // En az 1 birim boşluk
        if (plusTwoHealthPosition != Vector3.zero && Random.value < plusTwoHealthSpawnChance)
        {
            SpawnPlusTwoHealth(plusTwoHealthPosition);
        }

        // İşgal edilmiş pozisyonları temizle
        occupiedXPositions.Clear();
    }

    Vector3 GetAvailablePosition(float minDistance)
    {
        float xPosition = nextSpawnPosition.x;
        bool positionIsValid = false;
        int attempts = 0;

        // Uygun bir pozisyon bulana kadar dene
        while (!positionIsValid && attempts < 10)
        {
            xPosition = nextSpawnPosition.x + Random.Range(-platformWidth / 2, platformWidth / 2);
            positionIsValid = true;

            // İşgal edilmiş x konumlarını kontrol et
            foreach (float occupiedX in occupiedXPositions)
            {
                if (Mathf.Abs(xPosition - occupiedX) < minDistance)
                {
                    positionIsValid = false;
                    break;
                }
            }

            attempts++;
        }

        if (positionIsValid)
        {
            // Yeni konumu işgal edilmiş konumlar listesine ekle
            occupiedXPositions.Add(xPosition);
            return new Vector3(xPosition, nextSpawnPosition.y, nextSpawnPosition.z);
        }

        return Vector3.zero; // Geçerli bir konum yoksa sıfır döndür
    }

    void SpawnRock(Vector3 rockPosition)
    {
       
        rockPosition.y = manualRock.position.y;
        Instantiate(rockPrefab, rockPosition, Quaternion.identity);
    }

    void SpawnCoin(Vector3 coinPosition)
    {
       
        coinPosition.y = manualCoin.position.y;

        int coinCount = Random.Range(2, 5);
        for (int i = 0; i < coinCount; i++)
        {
            float xOffset = i * 1.5f;
            Vector3 coinPositionWithOffset = new Vector3(coinPosition.x + xOffset, coinPosition.y, coinPosition.z);
            Instantiate(coinPrefab, coinPositionWithOffset, Quaternion.identity);
        }
    }

    void SpawnCarrot(Vector3 CarrotPosition)
    {
       
        CarrotPosition.y = MevcutCarrotYPosition();
        Instantiate(CarrotPrefab, CarrotPosition, Quaternion.identity);
    }

    void SpawnPlusOneHealth(Vector3 position)
    {
        
        position.y = manualCoin.position.y;
        Instantiate(plusOneHealthPrefab, position, Quaternion.identity);
    }

    void SpawnPoison(Vector3 position)
    {
       
        position.y = manualCoin.position.y;
        Instantiate(poisonPrefab, position, Quaternion.identity);
    }

    void SpawnPlusTwoHealth(Vector3 position)
    {
        
        position.y = manualCoin.position.y;
        Instantiate(plusTwoHealthPrefab, position, Quaternion.identity);
    }

    float MevcutCarrotYPosition()
    {
        GameObject mevcutCarrot = GameObject.FindGameObjectWithTag("Carrot");
        if (mevcutCarrot != null)
        {
            return mevcutCarrot.transform.position.y;
        }
        return manualCoin.position.y;
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

        SpawnBalloonCluster(cloudPosition, cloudGroup);
    }

    void SpawnBalloonCluster(Vector3 cloudPosition, GameObject cloudGroup)
    {
        if (mathBalloonPrefab == null) return;

        float balloonY = cloudPosition.y - 1.7f;
        Vector3 balloonPosition = new Vector3(cloudPosition.x + 10f, balloonY, 0);

        List<int> answers = GenerateAnswerOptions();

        GameObject balloonGroup = new GameObject("BalloonGroup");
        balloonGroup.transform.SetParent(cloudGroup.transform);

        for (int i = 0; i < answers.Count; i++)
        {
            GameObject balloon = Instantiate(mathBalloonPrefab, balloonPosition, Quaternion.identity);
            balloon.transform.SetParent(balloonGroup.transform);
            AttachAnswerToBalloon(balloon, answers[i]);
            balloonPosition.x += 4f;
        }

        BalloonGroup balloonGroupScript = balloonGroup.AddComponent<BalloonGroup>();
    }

    void GenerateMathQuestion()
    {
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        int operation = Random.Range(0, 4);

        switch (operation)
        {
            case 0:
                currentMathQuestion = $"{a} + {b}";
                correctAnswer = a + b;
                break;
            case 1:
                if (a < b) { int temp = a; a = b; b = temp; }
                currentMathQuestion = $"{a} - {b}";
                correctAnswer = a - b;
                break;
            case 2:
                currentMathQuestion = $"{a} x {b}";
                correctAnswer = a * b;
                break;
            case 3:
                b = Mathf.Max(1, b);
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
            float playerSpeed = Mathf.Abs(playerRb.linearVelocity.x);
            spawnInterval = Mathf.Max(0.5f, 2f - playerSpeed * speedFactor);
        }
    }
}