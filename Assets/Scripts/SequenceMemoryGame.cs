using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using RTLTMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SequenceMemoryGame : MonoBehaviour
{
    [SerializeField] private Volume volume;
    private Vignette _vignette;
    
    [Header("Feedbacks")] 
    [SerializeField] private MMF_Player errorClickFeedback;
    [SerializeField] private MMF_Player levelUpFeedback;
    
    [Header("Starting Page")] 
    [SerializeField] private GameObject startingPage;

    [Header("Playing Page")] 
    [SerializeField] private GameObject playingPage;
    [SerializeField] private RTLTextMeshPro levelText;
    [SerializeField] private GameObject gridContainer;
    [SerializeField] private GameObject emptyGridRowPrefab;
    [SerializeField] private GameObject gridSquarePrefabInCanvas;
    [SerializeField] private GameObject gridSquarePrefabInWorld;
    [SerializeField] private int numOfRows;
    [SerializeField] private int numOfCols;

    [Header("Ending Page")] 
    [SerializeField] private GameObject endingPage;
    [SerializeField] private RTLTextMeshPro endingScoreText;

    public int currentLevel;

    public List<int> orderOfClicks = new List<int>();
    
    public bool inGame;
    public bool canPlay;

    private int _squareToClick;
    private int _orderIndex;

    public List<GameObject> gridSquaresInCanvas = new List<GameObject>();
    public List<GridSquare> squaresInWorld = new List<GridSquare>();

    // store grid square between mouse up and down to be able to light up and light down
    private GridSquare _gridSquareHit = null;
    
    // if we previously hit a grid, we can light it down
    private bool _previouslyHit = false;
    
    

    public static SequenceMemoryGame Instance; 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (inGame == false || canPlay == false)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Create a ray from the screen point
            Vector2 rayPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayPosition, Vector2.zero);

            // Check for collision with a 2D object
            if (hit.collider != null)
            {
                // Check if the collided object has a name
                // if (!string.IsNullOrEmpty(hit.collider.gameObject.name))
                // {
                //     Debug.Log("Touched Object Name: " + hit.collider.gameObject.name);
                // }
                
                _gridSquareHit = hit.transform.GetComponent<GridSquare>();
                _previouslyHit = true;
                _gridSquareHit.LightUp();
                // Debug.Log($"orderIndex: {orderIndex}");
                if (_gridSquareHit.order == orderOfClicks[_orderIndex])
                {
                    GetNextSquare();
                }
                else
                {
                    // handle losing game logic
                    Debug.Log("YOU LOST!");
                    StartCoroutine(LoseGame());
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse Button Up");
            
            if (_previouslyHit)
            {
                _gridSquareHit.LightDown();
                _previouslyHit = false;
            }
        }
    }

    private IEnumerator LoseGame()
    {
        // shake screen
        Camera.main.transform.DOShakePosition(0.2f, Vector3.one);
        AudioManager.Instance.PlayLostGame();
        errorClickFeedback?.PlayFeedbacks();
        yield return new WaitForSeconds(0.5f);

        // small red flash

        
        OpenEndingPage();
    }

    // Start is called before the first frame update
    void Start()
    {
        // _orderOfClicks.Add(5);
        // _orderOfClicks.Add(9);
        // foreach (var click in _orderOfClicks)
        // {
        //     Debug.Log(click);
        // }
        InitializeGrid(numOfRows,numOfCols);
    }

    // create 3 by 3 grid
    private void InitializeGrid(int row, int col)
    {
        for (int i = 0; i < row; i++)
        {
            GameObject gridRow = Instantiate(emptyGridRowPrefab, gridContainer.transform);
            for (int j = 0; j < col; j++)
            {
                GameObject square = Instantiate(gridSquarePrefabInCanvas, gridRow.transform);
                // GridSquare gridSq = square.GetComponent<GridSquare>();
                // gridSq.order = gridCounter;
                // gridCounter++;
                gridSquaresInCanvas.Add(square);
            }
        }
    }

    private IEnumerator InitializeGridInWorld()
    {
        yield return new WaitForSeconds(0.5f);
        // Waiting for the canvas to put them in the right position through "content size fitter" and other stuff
        // (cuz laying out takes time apparently)

        int gridCounter = 0;
        foreach (var sq in gridSquaresInCanvas)
        {
            RectTransform rectTransform = sq.GetComponent<RectTransform>();
            GameObject squareWorld = Instantiate(gridSquarePrefabInWorld, rectTransform.position, Quaternion.identity);
            GridSquare gridSq = squareWorld.GetComponent<GridSquare>();
            squaresInWorld.Add(gridSq);
            gridSq.order = gridCounter;
            gridCounter++;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void StartPlayingNewGame()
    {
        currentLevel = 0;
        OpenStartingPage();
        ResetGame();
        LevelUp();
    }

    private IEnumerator ShowSequence()
    {
        canPlay = false;
        // yield return new WaitForSeconds(0.5f);
        // Debug.Log(nameof(ShowSequence));
        // for (int i = 0; i < _squaresInWorld.Count; i++)
        // {
        //     for (int j = 0; j < _orderOfClicks.Count; j++)
        //     {
        //         if (_squaresInWorld[i].order == _orderOfClicks[j])
        //         {
        //             _squaresInWorld[i].FakeClick();
        //             break;
        //         }
        //     }
        //     yield return new WaitForSeconds(0.5f);
        // }

        yield return new WaitForSeconds(2f);
        for (int i = 0; i < orderOfClicks.Count; i++)
        {
            Debug.Log($"Count: {orderOfClicks.Count} - _orderOfClicks[{i}]: {orderOfClicks[i]}");
            int index = orderOfClicks[i];
            squaresInWorld[index].FakeClick();
            yield return new WaitForSeconds(0.5f);
        }

        canPlay = true;
    }
    
    private void CloseAllPages()
    {
        startingPage.SetActive(false);
        playingPage.SetActive(false);
        endingPage.SetActive(false);
    }

    public void OpenStartingPage()
    {
        CloseAllPages();
        startingPage.SetActive(true);
    }

    public void OpenPlayingPage()
    {
        CloseAllPages();
        playingPage.SetActive(true);
        StartCoroutine(InitializeGridInWorld());
        StartCoroutine(ShowSequence());
    }

    public void OpenEndingPage()
    {
        TurnOffGridInWorld();
        CloseAllPages();
        canPlay = false;
        inGame = false;
        endingScoreText.text = "مرحلة: "+currentLevel;
        endingPage.SetActive(true);
        orderOfClicks.Clear();
    }

    public void BackButton()
    {
        TurnOffGridInWorld();
        orderOfClicks.Clear();
        canPlay = false;
        inGame = false;
    }

    private void GetNextSquare()
    {
        _orderIndex++;

        if (_orderIndex+1 > orderOfClicks.Count)
        {
            StartCoroutine(LightDownLastSquare());
            LevelUp();
            StartCoroutine(ShowSequence());
        } 
    }

    private IEnumerator LightDownLastSquare()
    {
        yield return new WaitForSeconds(0.1f);
        _gridSquareHit.LightDown();
    }

    private void TurnOffGridInWorld()
    {
        foreach (var square in squaresInWorld)
        {
            Destroy(square.gameObject);
        }
        squaresInWorld.Clear();
    }

    public void ResetGame()
    {
        inGame = true;
    }

    public void LevelUp()
    {
        Debug.Log(nameof(LevelUp));
        currentLevel++;
        levelText.text = "مرحلة: "+ currentLevel;
        if (currentLevel > 1)
        {
            levelUpFeedback.PlayFeedbacks();
            AudioManager.Instance.PlayLevelUp();
        }
        orderOfClicks.Add(Random.Range(0,8));
        _orderIndex = 0;
    }
}
