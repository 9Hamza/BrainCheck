using System;
using System.Collections;
using System.Diagnostics;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ReactionTimeGame : MonoBehaviour
{
    [SerializeField] private float minimumTime;
    [SerializeField] private float maximumTime;
    
    [Header("UI")]
    [SerializeField] private Image background;
    [SerializeField] private Image mainImage;
    [SerializeField] private RTLTextMeshPro title;
    [SerializeField] private RTLTextMeshPro description;
    [SerializeField] private RTLTextMeshPro underDescription;

    [Header("String Values")] 
    
    
    [Header("Sprites")]
    [SerializeField] private Sprite boltIcon;
    [SerializeField] private Sprite threeDotsIcon;
    [SerializeField] private Sprite clockIcon;
    [SerializeField] private Sprite errorIcon;

    private bool _inGame;
    private CustomInput _customInput;
    private Color _originalBgColor;

    private float _elapsedTime;

    private bool _haveOpenWindowForErrorClick;

    // if you can start playing the game or not, depending on if you are in this page or not.
    public static bool CanStart = false;

    private bool _mouseIsOverUI;

    public Color _comfortGreen;
    public Color _comfortRed;

    private void Awake()
    {
        _comfortGreen = new Color32(89, 224, 105, 255);
        _comfortRed = new Color32(222, 14, 14, 255);

        // enable input
        _customInput = new CustomInput();
        _customInput.Enable();

        // cache original values
        _originalBgColor = background.color;

        ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanStart == false) return;
        if (Input.GetMouseButtonDown(0) && _inGame == false)
        {
            StartCoroutine(StartRound());
            StartCoroutine(WaitForErrorClick());
        }
    }

    private IEnumerator StartRound()
    {
        // ---- Start Round ----
        _inGame = true;
        
        // Wait for green
        _haveOpenWindowForErrorClick = true;
        background.color = _comfortRed;
        // background.color = Color.red;
        mainImage.sprite = threeDotsIcon;
        title.text = "انتظر اللون الأخضر";
        description.text = String.Empty;
        underDescription.text = String.Empty;
        yield return new WaitForSeconds(Random.Range(minimumTime, maximumTime));
        
        // Click!
        title.text = "اضغط!";
        background.color = _comfortGreen;
        // background.color = Color.green;

        _haveOpenWindowForErrorClick = false;
        
        // start timer
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        float time = 0f;
        while (time < 10)
        {
            // if user clicks, end round
            if (Input.GetMouseButtonDown(0))
            {
                // ---- End Round ----
                
                // stop the watch
                stopwatch.Stop();
                _elapsedTime = stopwatch.ElapsedMilliseconds;
                // Debug.Log(_elapsedTime);
                
                PlayFabManager.Instance.SendScoreToLeaderboard((int)_elapsedTime, PlayFabManager.GameType.ReactionTime);
                
                // change UI to round end state
                background.color = _originalBgColor;
                mainImage.sprite = clockIcon;
                title.text = $"{_elapsedTime} ms";
                description.text = "انقر للمحاول مجددا";
                underDescription.text = String.Empty;
                _inGame = false;
                _haveOpenWindowForErrorClick = false;
                yield break;
            }
            time += Time.deltaTime;
            yield return null;
        }
        _haveOpenWindowForErrorClick = false;
    }

    private IEnumerator WaitForErrorClick()
    {
        yield return null;
        while (_haveOpenWindowForErrorClick == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // update UI - Too Soon!
                background.color = _originalBgColor;
                mainImage.sprite = errorIcon;
                title.text = "مبكر جدا!";
                description.text = "حاول مرة أخرى!";
                underDescription.text = String.Empty;
                _inGame = false;
                
                // stop all coroutines
                StopAllCoroutines();
            }

            yield return null;
        }
    }

    public void ResetGame()
    {
        CanStart = false;
        _haveOpenWindowForErrorClick = false;
        _inGame = false;
        title.text = "اختبر سرعة ردة فعلك";
        description.text = "عندما يتحول اللون الأحمر إلى اللون الأخضر ، انقر بأسرع ما يمكن";
        underDescription.text = "انقر في أي مكان للبدء";
        background.color = _originalBgColor;
        mainImage.sprite = boltIcon;
        StopAllCoroutines();
    }

    // Doesn't work (idk this always returns true. Prob because the canvas is stretched and is being used as playground)
    // private void MouseIsOverUI()
    // {
    //     _mouseIsOverUI = EventSystem.current.IsPointerOverGameObject();
    //     Debug.Log(_mouseIsOverUI);
    // }
}
