using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDInGame : MonoBehaviour {

    public const string pugMsg = "Remaining pugs: {0:000}";
    public const string korokkeMsg= "Korokkes left: {0:000}";

    public Text pugCounter;
    public Text korokkeCounter;

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
    }

    private void OnDestroy()
    {
        gpManager.OnGameStarted -= UpdateUI;
        gpManager.OnPugHit -= OnPugCounterChanged;
        gpManager.OnKorokkeStolen -= OnKorokkeCounterChanged;
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
}
