using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDInGame : MonoBehaviour {

    public const string pugMsg = "Remaining pugs: {0:000}";
    public const string korokkeMsg= "Korokkes left: {0:000}";
    public const string beatGameVictoryPopup = "The pugs are gone for good this time!";
    public const string defaultVictoryPopup = "You've scared the pugs away{0}! Ready for their next attempt?";
    public const string extraDefaultPopup = " and kept all the korokkes";
    public const string levelTitle = "Level {0}: {1}";

    public const string perfectVictoryTitle = "Brilliant!";
    public const string defaultVictoryTitle = "Well done!";

    public const string defaultButtonWin = "Of course!";
    public const string beatGameButtonWin = "Awesome!";


    public Text pugCounter;
    public Text korokkeCounter;

    public RectTransform popupWin;
    public Button buttonWin;
    public Text winDesc;
    public Text winTitle;
    public Text buttonWinLabel;

    public RectTransform popupLose;
    public Button buttonLose;

    public RectTransform popupEnd;
    public Button buttonEnd;

    public RectTransform popupNewLevel;
    public float newLevelTime = 2.0f;
    public Text levelPopupTitle;
    public Text levelPopupDesc;

    GameplayManager gpManager;

    void Awake()
    {
        gpManager = FindObjectOfType<GameplayManager>();
    }

    private void Start()
    {
        gpManager.OnGameStarted += GameStart;
        gpManager.OnPugHit += OnPugCounterChanged;
        gpManager.OnKorokkeStolen += OnKorokkeCounterChanged;
        gpManager.OnGameWon += ShowVictoryPopup;
        gpManager.OnGameLost += ShowDefeatPopup;
        gpManager.OnLevelExit += OnLevelExited;
        gpManager.OnGameOver += ShowGameOverPopup;
    }

    private void OnDestroy()
    {
        gpManager.OnGameStarted -= GameStart;
        gpManager.OnPugHit -= OnPugCounterChanged;
        gpManager.OnKorokkeStolen -= OnKorokkeCounterChanged;
        gpManager.OnGameWon -= ShowVictoryPopup;
        gpManager.OnGameLost -= ShowDefeatPopup;
        gpManager.OnLevelExit -= OnLevelExited;
    }

    void GameStart(int pugs, int korokke)
    {
        HideVictoryPopup();
        HideDefeatPopup();

        OnPugCounterChanged(pugs);
        OnKorokkeCounterChanged(korokke);
        ShowLevelIntroPopup();
    }

    public void OnPugCounterChanged(int remaining)
    {
        pugCounter.text = string.Format(pugMsg, remaining);
    }

    public void OnKorokkeCounterChanged(int remaining)
    {
        korokkeCounter.text = string.Format(korokkeMsg, remaining);
    }

    public void ShowLevelIntroPopup()
    {
        StartCoroutine(ShowLevelIntroCoroutine(gpManager.levelIdx, gpManager.levelTitle, gpManager.levelDesc));
    }

    IEnumerator ShowLevelIntroCoroutine(int idx, string title, string levelDesc)
    {
        popupNewLevel.gameObject.SetActive(true);
        levelPopupTitle.text = string.Format(levelTitle, idx, title);
        levelPopupDesc.text = levelDesc;
        yield return new WaitForSeconds(this.newLevelTime);
        popupNewLevel.gameObject.SetActive(false);
        gpManager.Run();
    }

    public void ShowGameOverPopup()
    {
        popupEnd.gameObject.SetActive(true);
        buttonEnd.onClick.AddListener(OnGameOverClicked);
    }

    public void ShowVictoryPopup(bool perfect)
    {
        popupWin.gameObject.SetActive(true);

        if (gpManager.LastLevel)
        {
            winDesc.text = beatGameVictoryPopup;
            buttonWinLabel.text = beatGameButtonWin;
        }
        else
        {
            winDesc.text = string.Format(defaultVictoryPopup, perfect ? extraDefaultPopup : "");
            buttonWinLabel.text = defaultButtonWin;
            winTitle.text = perfect ? perfectVictoryTitle : defaultVictoryTitle;
        }
        
        buttonWin.onClick.AddListener(OnWinButtonClicked);
    }

    public void HideVictoryPopup()
    {
        popupWin.gameObject.SetActive(false);
        buttonWin.onClick.RemoveAllListeners();
    }
    public void ShowDefeatPopup()
    {
        popupLose.gameObject.SetActive(true);
        buttonLose.onClick.AddListener(OnLoseButtonClicked);
    }

    public void HideDefeatPopup()
    {
        popupLose.gameObject.SetActive(false);
        buttonLose.onClick.RemoveAllListeners();
    }

    public void OnLevelExited()
    {
        // Prevent additional clicks
        buttonWin.onClick.RemoveAllListeners();
        buttonLose.onClick.RemoveAllListeners();
    }

    public void OnWinButtonClicked()
    {
        gpManager.NextLevel();
    }
    public void OnLoseButtonClicked()
    {
        gpManager.RepeatLevel();
    }

    public void OnGameOverClicked()
    {
        gpManager.ResetGame();
    }
}
