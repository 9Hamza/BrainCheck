using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using RTLTMPro;
using Random = UnityEngine.Random;

public class WordLoader : MonoBehaviour
{
    HashSet<string> _wordSet;

    private HashSet<string> _newWords;
    private HashSet<string> _seenWords;

    [SerializeField] private int lives;

    [Header("Starting Page")] 
    [SerializeField] private GameObject startingPage;

    [Header("Playing Page")] 
    [SerializeField] private GameObject playingPage;
    [SerializeField] private RTLTextMeshPro mainWordText;
    [SerializeField] private RTLTextMeshPro scoreText;
    [SerializeField] private RTLTextMeshPro livesText;

    [Header("Ending Page")] 
    [SerializeField] private GameObject endingPage;
    [SerializeField] private RTLTextMeshPro endingScoreText;

    private string _currentWord;
    private int _score;
    private int _maxLives;

    private Vector3 _textOriginalLocalScale;

    private void Awake()
    {
        _seenWords = new HashSet<string>();
        _newWords = new HashSet<string>();
        _maxLives = lives;

        _textOriginalLocalScale = mainWordText.transform.localScale;
    }

    void Start()
    {
        // string csvFilePath = "Assets/Resources/input.csv"; // Specify the path to your CSV file
        // TextAsset inputFile = Resources.Load<TextAsset>("input");
        
        // Find the file path of the CSV file in the Resources folder
        // string filePath = FindCSVFilePath("input");
        string filePath = Application.streamingAssetsPath + "/input.csv";
        // Debug.Log(filePath);

        LoadWordsFromCSV(filePath);

        StartPlayingNewGame();
    }

    /*private string FindCSVFilePath(string fileName)
    {
        // Search for the CSV file in the Resources folder
        string[] filePaths = Directory.GetFiles(Application.dataPath + "/Resources", fileName + ".csv", SearchOption.AllDirectories);

        if (filePaths.Length > 0)
        {
            // Return the first found file path
            return filePaths[0];
        }

        return null;
    }*/
    
    void LoadWordsFromCSV(string filePath)
    {
        _wordSet = new HashSet<string>();

        try
        {
            // Read the CSV file
            string[] csvLines = File.ReadAllLines(filePath);

            // Read words from each line
            foreach (string line in csvLines)
            {
                string[] words = line.Split(',');

                // Add each word to the HashSet
                foreach (string word in words)
                {
                    string trimmedWord = word.Trim();

                    // Add the word to the HashSet
                    _wordSet.Add(trimmedWord);
                }
            }

            Debug.Log("Words loaded successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading words: " + e.Message);
        }
    }

    string GetRandomNewWord()
    {
        if (_wordSet.Count == 0)
        {
            Debug.LogWarning("The main hashset is empty.");
            return null;
        }
        
        int randomIndex = Random.Range(0, _newWords.Count);
        string randomWord = _newWords.ElementAt(randomIndex);
        _newWords.Remove(randomWord);
        // Debug.Log(randomWord);
        
        return randomWord;
    }

    string GetRandomSeenWord()
    {
        if (_seenWords.Count > 0)
        {
            int randomIndex = Random.Range(0, _seenWords.Count);
            string randomWord = _seenWords.ElementAt(randomIndex);
            
            Debug.Log(randomWord);
        
            return randomWord;
        }
        else
        {
            return GetRandomNewWord();
        }
    }

    private void LoadNextWord()
    {
        string randomWord;
        float probabilityNum = Random.Range(0f, 1f);
        // Debug.Log(probabilityNum);
        if (probabilityNum > 0.5f)
        {
            randomWord = GetRandomNewWord();
        }
        else
        {
            randomWord = GetRandomSeenWord();
        }
        
        mainWordText.transform.localScale = Vector3.zero;
        mainWordText.text = randomWord;
        mainWordText.transform.DOScale(_textOriginalLocalScale, 0.1f);

        // foreach (var wo in _seenWords)
        // {
        //     Debug.Log(wo);
        // }    
    }

    public void WordIsSeen()
    {
        // string myWord = mainWordText.text;
        // new - check if word is in seen words
        foreach (var word in _seenWords)
        {
            if (String.Equals(mainWordText.OriginalText, word))
            {
                _score++;
                AudioManager.Instance.PlayCorrectClick();
                UpdateStats();
                LoadNextWord();
                return;

            }
        }
        DecrementLives();
        UpdateStats();
        LoadNextWord();
    }

    public void WordIsNew()
    {
        // seen - check if word is in seen words
        foreach (var word in _seenWords)
        {
            Debug.Log($"currentWord: {mainWordText.OriginalText} ? seenWord: {word}");
            if (String.Equals(mainWordText.OriginalText,word))
            {
                DecrementLives();
                UpdateStats();
                LoadNextWord();
                return;
            }
        }
        _seenWords.Add(mainWordText.OriginalText);
        _score++;
        AudioManager.Instance.PlayCorrectClick();
        UpdateStats();
        LoadNextWord();
    }

    public void DecrementLives()
    {
        lives--;
        AudioManager.Instance.PLayIncorrectClick();
        if (lives <= 0)
        {
            // load ending page
            EndGame();
        }
    }

    private HashSet<string> CreateRandomSubset(HashSet<string> originalSet, int subsetSize)
    {
        HashSet<string> subset = new HashSet<string>();

        List<string> wordList = new List<string>(originalSet);

        // Ensure the subset size does not exceed the total number of words
        subsetSize = Mathf.Clamp(subsetSize, 0, wordList.Count);

        // Select random words from the original set
        while (subset.Count < subsetSize)
        {
            int randomIndex = Random.Range(0, wordList.Count);
            string randomWord = wordList[randomIndex];
            subset.Add(randomWord);
            wordList.RemoveAt(randomIndex);
        }

        return subset;
    }

    private void ResetGame()
    {
        _score = 0;
        lives = _maxLives;
        _newWords.Clear();
        _newWords = CreateRandomSubset(_wordSet, _wordSet.Count / 2);
        _seenWords.Clear();
        UpdateStats();
    }

    public void StartPlayingNewGame()
    {
        OpenStartingPage();
        ResetGame();
        LoadNextWord();
    }

    public void EndGame()
    {
        string kalemah;
        if (_score < 10)
        {
            kalemah = " كلمات ";
        }
        else
        {
            kalemah = " كلمة ";
        }
        endingScoreText.text = _score + kalemah ;
        OpenEndingPage();
    }

    private void UpdateStats()
    {
        scoreText.text = _score.ToString();
        livesText.text = lives.ToString();
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
    }

    public void OpenEndingPage()
    {
        CloseAllPages();
        endingPage.SetActive(true);
    }
}