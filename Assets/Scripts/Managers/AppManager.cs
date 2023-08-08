using DG.Tweening;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    
    [Header("Views")] 
    [SerializeField] private CanvasGroup viewMainMenu;
    [SerializeField] private CanvasGroup viewReactionTimeGame;
    [SerializeField] private CanvasGroup viewAimTrainerGame;
    [SerializeField] private CanvasGroup viewVerbalMemoryGame;
    [SerializeField] private CanvasGroup viewSequenceMemoryGame;

    [Header("Bottom Nav Bar")] [SerializeField]
    private GameObject bottomNavBar;
    
    [Header("Bottom Nav Bar Sprites")] 
    [SerializeField] private Sprite puzzleFilled;
    [SerializeField] private Sprite leaderboardFilled;
    [SerializeField] private Sprite profileFilled;

    public enum View
    {
        MainMenu,
        ReactionTime,
        AimTrainer,
        VerbalMemory,
        SequenceMemory
    }

    private View _currentView = View.MainMenu;
    private CanvasGroup _currentViewCg;

    private void Awake()
    {
        InitializeApp();
    }

    public void OpenView(View view)
    {
        Debug.Log(nameof(OpenView));
        Debug.Log(_currentView);
        // If we are in that view, no need to open it
        if (_currentView == view) return;

        // View to open
        CanvasGroup viewCanvasGroup;
        switch (view)
        {
            case View.MainMenu:
                viewCanvasGroup = viewMainMenu;
                break;
            case View.ReactionTime:
                viewCanvasGroup = viewReactionTimeGame;
                break;
            case View.AimTrainer:
                viewCanvasGroup = viewAimTrainerGame;
                break;
            case View.VerbalMemory:
                viewCanvasGroup = viewVerbalMemoryGame;
                break;
            case View.SequenceMemory:
                viewCanvasGroup = viewSequenceMemoryGame;
                break;
            default:
                viewCanvasGroup = null;
                break;
        }
        
        _currentViewCg = GetCurrentView(_currentView);
        ShowCanvasGroup(false, _currentViewCg);
        ShowCanvasGroup(true, viewCanvasGroup);
    }

    private CanvasGroup GetCurrentView(View view)
    {
        switch (view)
        {
            case View.MainMenu:
                return viewMainMenu;
            case View.ReactionTime:
                return viewReactionTimeGame;
            case View.AimTrainer:
                return viewAimTrainerGame;
            case View.VerbalMemory:
                return viewVerbalMemoryGame;
            case View.SequenceMemory:
                return viewSequenceMemoryGame;
            default:
                return null;
        }
    }

    public void OpenMainMenu()
    {
        OpenView(View.MainMenu);
        _currentView = View.MainMenu;
    }

    public void OpenAimTrainerGame()
    {
        OpenView(View.AimTrainer);
        _currentView = View.AimTrainer;
    }

    public void OpenReactionTime()
    {
        OpenView(View.ReactionTime);
        _currentView = View.ReactionTime;
        ReactionTimeGame.CanStart = true;
    }

    public void OpenVerbalMemoryGame()
    {
        OpenView(View.VerbalMemory);
        _currentView = View.VerbalMemory;
    }

    public void OpenSequenceMemoryGame()
    {
        OpenView(View.SequenceMemory);
        _currentView = View.SequenceMemory;
    }

    public void InitializeApp()
    {
        CanvasGroup[] canvasGroups = GameObject.Find("Canvas").GetComponentsInChildren<CanvasGroup>();
        foreach (var canvasGroup in canvasGroups)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        ShowCanvasGroup(true, viewMainMenu);
    }

    private void ShowCanvasGroup(bool show, CanvasGroup canvasGroup)
    {
        if (show)
        {
            canvasGroup.DOFade(1, 0.5f);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.DOFade(0, 0.5f);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
