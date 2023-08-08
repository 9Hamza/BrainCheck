using System.Diagnostics;
using DG.Tweening;
using RTLTMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class AimTrainerGame : MonoBehaviour
{
    [Header("Starting Page")]
    [SerializeField] private RectTransform startingPage;
    
    [Header("In Game Page")]
    [SerializeField] private RectTransform inProgressPage;
    [SerializeField] private RTLTextMeshPro remainingTargetsText;
    [SerializeField] private RectTransform topPanel;

    [Header("Ending Page")] 
    [SerializeField] private RectTransform endingPage;
    [SerializeField] private RTLTextMeshPro elapsedTimeText;

    [Space]
    [SerializeField] private int remainingTargets;
    [SerializeField] private GameObject prefabToSpawn;

    private Stopwatch _stopwatch;
    private bool _stopWatchStarted;
    private float _elapsedTime;
    private int _maxRemainingTargets;

    private float _topOfScreen; // the position of the top panel (which has pivot of y as 0)
    private float _rightOfScreen; // the max right of screen (which 
    
    public static bool CanStart = false;
    public static AimTrainerGame Instance;
    private void Awake()
    {
        _maxRemainingTargets = remainingTargets;
        _stopwatch = new Stopwatch();
        
        startingPage.gameObject.SetActive(true);
        inProgressPage.gameObject.SetActive(false);
        endingPage.gameObject.SetActive(false);

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector2 panelPosInScreenSpace = RectTransformUtility.WorldToScreenPoint(Camera.main, topPanel.position);
        _topOfScreen = panelPosInScreenSpace.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanStart == false) return;
        ShootBullseye();
    }

    private void ShootBullseye()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin,ray.direction);
            
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null && hit.collider.CompareTag("target"))
            {
                Debug.Log("Hit a target object!");
                // Do something when the ray hits a target object
                remainingTargets--;
                UpdateRemainingTargetsText();
                // hit.transform.DOScale(Vector3.zero, 0.1f);
                Destroy(hit.transform.gameObject);
                SpawnPrefab();
                UpdateStopwatch();
                CheckIfRoundFinish();
            }
        }
    }

    private void SpawnPrefab()
    {
        if (remainingTargets <= 0) return;
        
        Vector2 randomPoint = GetRandomPointInScreen();
        Debug.Log(randomPoint);
        Vector3 spawnPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(inProgressPage,randomPoint,Camera.main, out spawnPoint);
        spawnPoint.z = 0;
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPoint, Quaternion.identity);
        Vector3 originalLocalScale = spawnedObject.transform.localScale;
        spawnedObject.transform.localScale = Vector3.zero;
        spawnedObject.SetActive(true);
        spawnedObject.transform.DOScale(originalLocalScale, 0.1f);
    }

    private Vector2 GetRandomPointInScreen()
    {
        float minY = 200;
        float maxY = _topOfScreen;
        float minX = 0;
        float maxX = Screen.width;

        Vector2 randomPositionInScreenSpace = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        return randomPositionInScreenSpace;
    }

    private void CheckIfRoundFinish()
    {
        if (remainingTargets <= 0)
        {
            // close game
            CanStart = false;
            
            // open ending page
            endingPage.gameObject.SetActive(true);
            startingPage.gameObject.SetActive(false);
            inProgressPage.gameObject.SetActive(false);
            
            // update ending page ui
            UpdateStopwatch();
            elapsedTimeText.text = $"{(int)_elapsedTime/_maxRemainingTargets} ms / طلقه";
        }
    }

    private void UpdateStopwatch()
    {
        if (remainingTargets<_maxRemainingTargets && _stopWatchStarted == false)
        {
            _stopWatchStarted = true;
            _stopwatch.Start();
        }
        else if (remainingTargets <= 0)
        {
            _stopwatch.Stop();
            _elapsedTime = _stopwatch.ElapsedMilliseconds;
            Debug.Log(_elapsedTime);
        }
    }

    // when player clicks first bullseye button, start the round
    public void StartRound()
    {
        CanStart = true;
        remainingTargets = _maxRemainingTargets;
        UpdateRemainingTargetsText();
        startingPage.gameObject.SetActive(false);
        inProgressPage.gameObject.SetActive(true);
        endingPage.gameObject.SetActive(false);
        SpawnPrefab();
    }

    // when player shoots the bullseye, update the UI with remaining text
    private void UpdateRemainingTargetsText()
    {
        remainingTargetsText.text = remainingTargets.ToString();
    }

    public void LeaveGame()
    {
        CanStart = false;
        GameObject[] targets = GameObject.FindGameObjectsWithTag("target");
        foreach (var target in targets)
        {
            Destroy(target);
        }
    }
}
