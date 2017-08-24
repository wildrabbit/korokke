using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Config")]
    public Transform sceneRoot;
    public Vector3 korokkePosition;

    public Transform[] pugSpawnPoints;

    [Header("Prefabs")]
    public Korokke korokkePrefab;
    public Pug pugPrefab;

    //------------------------
    Korokke korokke;
    List<Pug> pugs;
	// Use this for initialization
	void Start ()
    {
        BuildLevel();
        StartGame();
	}

    void StartGame()
    {
        korokke.StartGame();
        foreach(Pug p in pugs)
        {
            p.StartGame();
        }
    }
	
	// Update is called once per frame
	void Update () {
        float delta = Time.deltaTime;
        korokke.LogicUpdate(delta);
        foreach (Pug p in pugs)
        {
            p.LogicUpdate(delta);
        }
	}

    void StopGame()
    {
        korokke.StopGame();
        foreach(Pug p in pugs)
        {
            p.StopGame();
        }
    }

    void BuildLevel()
    {
        korokke = Instantiate<Korokke>(korokkePrefab);
        korokke.Init(this);

        Pug[] existingPugs = FindObjectsOfType<Pug>();
        pugs = new List<Pug>(existingPugs);
        foreach (Pug p in pugs)
        {
            p.Init(this);
        }

        existingPugs = null;
    }


    void ExitLevel()
    {
        korokke.Cleanup();
        korokke.Kill();

        foreach (Pug p in pugs)
        {
            p.Cleanup();
            p.Kill();
        }
    }
}
