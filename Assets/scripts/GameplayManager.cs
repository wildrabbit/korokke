using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static int NextPugID = 0;

    [Header("Config")]
    public Transform sceneRoot;
    public Transform pugRoot;
    public Vector3 korokkePosition;
    public int maxPugs = 30;
    public float minSpawnDelay = 0.2f;
    public float maxSpawnDelay = 0.8f;

    public Transform[] pugSpawnPoints;

    [Header("Prefabs")]
    public Korokke korokkePrefab;
    public Pug pugPrefab;

    //------------------------
    float[] elapsedSpawners;
    float[] spawnDelays;

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

        for (int i = 0; i < pugSpawnPoints.Length; ++i)
        {
            elapsedSpawners[i] = 0.0f;
            spawnDelays[i] = Random.Range(minSpawnDelay, maxSpawnDelay);
        }

        foreach(Pug p in pugs)
        {
            p.StartGame();
        }
    }
	
	// Update is called once per frame
	void Update () {
        float delta = Time.deltaTime;
        korokke.LogicUpdate(delta);

        // Update spawns
        UpdateSpawns(delta);

        foreach (Pug p in pugs)
        {
            p.LogicUpdate(delta);
        }
	}

    void UpdateSpawns(float delta)
    {
        if (pugs.Count >= maxPugs) return;
        for (int i = 0; i < pugSpawnPoints.Length; ++i)
        {
            elapsedSpawners[i] += delta;
            if (elapsedSpawners[i] >= spawnDelays[i])
            {
                Pug p = Instantiate<Pug>(pugPrefab);
                p.Init(this);
                p.boidData.pos = pugSpawnPoints[i].position;
                p.StartGame();
                pugs.Add(p);

                elapsedSpawners[i] = 0;
                spawnDelays[i] = Random.Range(minSpawnDelay, maxSpawnDelay);

                NextPugID++;

                if (pugs.Count == maxPugs) break;
            }
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
        int numPoints = pugSpawnPoints.Length;
        elapsedSpawners = new float[numPoints];
        spawnDelays = new float[numPoints];

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

    public Vector2 GetKorokkePosition()
    {
        return korokke.transform.position;
    }
}
