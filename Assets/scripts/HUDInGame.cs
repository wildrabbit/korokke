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

    public float defaultSoundLevel = 0.4f;

    public Sprite soundOn;
    public Sprite soundOff;

    public Image iconBtnSound;
    public Button btnSound;

    public RectTransform pugCounterPanel;
    public Text pugCounter;

    public RectTransform korokkeCounterPanel;
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

    public AudioClip soundButtonClicked;

    public RectTransform popupNewLevel;
    public float newLevelTime = 2.0f;
    public Text levelPopupTitle;
    public Text levelPopupDesc;

    GameplayManager gpManager;
    AudioSource audioSrc;

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
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

        btnSound.onClick.AddListener(ToggleSoundClicked);
        RefreshButtonIcon();        
    }

    private void OnDestroy()
    {
        gpManager.OnGameStarted -= GameStart;
        gpManager.OnPugHit -= OnPugCounterChanged;
        gpManager.OnKorokkeStolen -= OnKorokkeCounterChanged;
        gpManager.OnGameWon -= ShowVictoryPopup;
        gpManager.OnGameLost -= ShowDefeatPopup;
        gpManager.OnLevelExit -= OnLevelExited;

        btnSound.onClick.RemoveAllListeners();
    }

    void ToggleSoundClicked()
    {
        AudioListener.volume = Mathf.Approximately(AudioListener.volume, 0.0f) ? defaultSoundLevel : 0.0f;
        audioSrc.PlayOneShot(soundButtonClicked);
        RefreshButtonIcon();
    }

    void RefreshButtonIcon()
    {
        iconBtnSound.sprite = Mathf.Approximately(AudioListener.volume, 0.0f) ? soundOff : soundOn;
    }

    void GameStart(int pugs, int korokke)
    {
        HideVictoryPopup();
        HideDefeatPopup();
        btnSound.gameObject.SetActive(false);

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
        pugCounterPanel.gameObject.SetActive(false);
        korokkeCounterPanel.gameObject.SetActive(false);
        StartCoroutine(ShowLevelIntroCoroutine(gpManager.levelIdx, gpManager.levelTitle, gpManager.levelDesc));
    }

    IEnumerator ShowLevelIntroCoroutine(int idx, string title, string levelDesc)
    {
        popupNewLevel.gameObject.SetActive(true);
        levelPopupTitle.text = string.Format(levelTitle, idx, title);
        levelPopupDesc.text = levelDesc;
        yield return new WaitForSeconds(this.newLevelTime);
        popupNewLevel.gameObject.SetActive(false);
        pugCounterPanel.gameObject.SetActive(true);
        korokkeCounterPanel.gameObject.SetActive(true);
        btnSound.gameObject.SetActive(true);
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
        audioSrc.PlayOneShot(soundButtonClicked);
        gpManager.NextLevel();
    }
    public void OnLoseButtonClicked()
    {
        audioSrc.PlayOneShot(soundButtonClicked);
        gpManager.RepeatLevel();
    }

    public void OnGameOverClicked()
    {
        audioSrc.PlayOneShot(soundButtonClicked);
        gpManager.ResetGame();
    }
}
