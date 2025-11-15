using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 게임 시작 시 자동으로 모든 오브젝트를 생성하고 설정
/// </summary>
public class GameSetup : MonoBehaviour
{
    private static bool initialized = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Initialize()
    {
        if (initialized) return;
        initialized = true;

        GameObject setupObj = new GameObject("GameSetup");
        setupObj.AddComponent<GameSetup>();
    }

    void Awake()
    {
        SetupCamera();
        SetupUI();
        SetupGameObjects();
    }

    void SetupCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }

        mainCam.transform.position = new Vector3(0, 0, -10);
        mainCam.orthographic = true;
        mainCam.orthographicSize = 5;
        mainCam.backgroundColor = new Color(0.05f, 0.05f, 0.1f);
    }

    void SetupUI()
    {
        // Canvas 생성
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Status Text (상태 메시지)
        GameObject statusTextObj = new GameObject("StatusText");
        statusTextObj.transform.SetParent(canvasObj.transform);
        Text statusText = statusTextObj.AddComponent<Text>();
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.fontSize = 32;
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.color = Color.white;
        RectTransform statusRect = statusTextObj.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.5f, 0.7f);
        statusRect.anchorMax = new Vector2(0.5f, 0.7f);
        statusRect.sizeDelta = new Vector2(600, 100);
        statusRect.anchoredPosition = Vector2.zero;

        // Timer Text (타이머)
        GameObject timerTextObj = new GameObject("TimerText");
        timerTextObj.transform.SetParent(canvasObj.transform);
        Text timerText = timerTextObj.AddComponent<Text>();
        timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        timerText.fontSize = 48;
        timerText.alignment = TextAnchor.MiddleCenter;
        timerText.color = new Color(1f, 0.5f, 0.5f);
        RectTransform timerRect = timerTextObj.GetComponent<RectTransform>();
        timerRect.anchorMin = new Vector2(0.5f, 0.9f);
        timerRect.anchorMax = new Vector2(0.5f, 0.9f);
        timerRect.sizeDelta = new Vector2(300, 80);
        timerRect.anchoredPosition = Vector2.zero;

        // Score Text (점수)
        GameObject scoreTextObj = new GameObject("ScoreText");
        scoreTextObj.transform.SetParent(canvasObj.transform);
        Text scoreText = scoreTextObj.AddComponent<Text>();
        scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        scoreText.fontSize = 24;
        scoreText.alignment = TextAnchor.UpperLeft;
        scoreText.color = Color.white;
        RectTransform scoreRect = scoreTextObj.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0f, 1f);
        scoreRect.anchorMax = new Vector2(0f, 1f);
        scoreRect.sizeDelta = new Vector2(300, 50);
        scoreRect.anchoredPosition = new Vector2(20, -20);

        // Instruction Text (조작법)
        GameObject instructionTextObj = new GameObject("InstructionText");
        instructionTextObj.transform.SetParent(canvasObj.transform);
        Text instructionText = instructionTextObj.AddComponent<Text>();
        instructionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        instructionText.fontSize = 20;
        instructionText.alignment = TextAnchor.LowerCenter;
        instructionText.color = new Color(0.7f, 0.7f, 0.7f);
        instructionText.text = "Click and drag to slash through magic circles!";
        RectTransform instructionRect = instructionTextObj.GetComponent<RectTransform>();
        instructionRect.anchorMin = new Vector2(0.5f, 0f);
        instructionRect.anchorMax = new Vector2(0.5f, 0f);
        instructionRect.sizeDelta = new Vector2(600, 50);
        instructionRect.anchoredPosition = new Vector2(0, 30);

        // Store references
        GameObject.Find("GameManager").GetComponent<GameManager>().statusText = statusText;
        GameObject.Find("GameManager").GetComponent<GameManager>().timerText = timerText;
        GameObject.Find("GameManager").GetComponent<GameManager>().scoreText = scoreText;
    }

    void SetupGameObjects()
    {
        // GameManager 오브젝트
        GameObject gameManagerObj = new GameObject("GameManager");
        GameManager gameManager = gameManagerObj.AddComponent<GameManager>();
        gameManager.castTime = 3f;
        gameManager.cutsNeededToWin = 3;
        gameManager.spawnPosition = Vector2.zero;

        // MagicCircle Prefab 생성 (프리팹처럼 사용할 원본)
        GameObject circlePrefabObj = new GameObject("MagicCirclePrefab");
        MagicCircle circlePrefab = circlePrefabObj.AddComponent<MagicCircle>();
        circlePrefab.drawDuration = 3f;
        circlePrefab.circleColor = new Color(0.5f, 0.2f, 1f, 1f);
        circlePrefab.lineWidth = 0.1f;
        circlePrefabObj.SetActive(false); // 프리팹으로 사용
        gameManager.magicCirclePrefab = circlePrefab;

        // SlashDetector 오브젝트
        GameObject slashDetectorObj = new GameObject("SlashDetector");
        SlashDetector slashDetector = slashDetectorObj.AddComponent<SlashDetector>();
        slashDetector.slashWidth = 0.15f;
        slashDetector.slashColor = new Color(1f, 0.3f, 0.3f, 1f);
        slashDetector.trailDuration = 0.3f;
        gameManager.slashDetector = slashDetector;

        Debug.Log("Game setup complete! All objects created.");
    }
}
