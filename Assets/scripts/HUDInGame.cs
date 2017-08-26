using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDInGame : MonoBehaviour {

    public const string pugMsg = "Remaining pugs: {0:000}";
    public const string korokkeMsg= "Korokkes left: {0:000}";

    public Text pugCounter;
    public Text korokkeCounter;

    public RectTransform popupWin;
    public Button buttonWin;

    public RectTransform popupLose;
    public Button buttonLose;

    GameplayManager gpManager;

    void Awake()
    {
        gpManager = FindObjectOfType<GameplayManager>();
    }

    private void Start()
    {
        gpManager.OnGameStarted += UpdateUI;
        gpManager.OnPugHit += OnPugCounterChanged;
        gpManager.OnKorokkeStolen += OnKorokkeCounterChanged;
        gpManager.OnGameWon += ShowVictoryPopup;
        gpManager.OnGameLost += ShowDefeatPopup;
    }

    private void OnDestroy()
    {
        gpManager.OnGameStarted -= UpdateUI;
        gpManager.OnPugHit -= OnPugCounterChanged;
        gpManager.OnKorokkeStolen -= OnKorokkeCounterChanged;
        gpManager.OnGameWon -= ShowVictoryPopup;
        gpManager.OnGameLost -= ShowDefeatPopup;
    }

    void UpdateUI(int pugs, int korokke)
    {
        OnPugCounterChanged(pugs);
        OnKorokkeCounterChanged(korokke);
    }

    public void OnPugCounterChanged(int remaining)
    {
        pugCounter.text = string.Format(pugMsg, remaining);
    }

    public void OnKorokkeCounterChanged(int remaining)
    {
        korokkeCounter.text = string.Format(korokkeMsg, remaining);
    }

    public void ShowVictoryPopup()
    {
        popupWin.gameObject.SetActive(true);
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

    public void OnWinButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    public void OnLoseButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
