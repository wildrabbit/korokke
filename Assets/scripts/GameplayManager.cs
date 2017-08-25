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

    public float spawnerRadius = 0.15f;
    public float maxSpeedPercentage = 2.0f;
    public float boidJitter = 0.1f;
    public float boidDistance = 2.0f;
    public float boidRadius = 1.0f;
    //The heading is the target.
    
    [Header("Prefabs")]
    public Korokke korokkePrefab;
    public Pug pugPrefab;

    Korokke korokke;
    List<Pug> pugs;
    List<Spawner> spawners;

    List<IEntity> entitiesToRemove;

	// Use this for initialization
	void Start ()
    {
        BuildLevel();
        StartGame();
	}

    void StartGame()
    {
        korokke.StartGame();

        foreach(Spawner sp in spawners)
        {
            sp.StartGame();
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
        UpdateSpawners(delta);

        foreach (Pug p in pugs)
        {
            p.LogicUpdate(delta);
        }

        for (int i = 0; i < entitiesToRemove.Count; ++i)
        {
            if (entitiesToRemove[i] is Pug)
            {
                pugs.Remove((Pug)entitiesToRemove[i]);
            }
            else if (entitiesToRemove[i] is Spawner)
            {
                spawners.Remove((Spawner)entitiesToRemove[i]);
            }
            entitiesToRemove[i].Kill();
        }
        entitiesToRemove.Clear();
	}

    void UpdateSpawners(float delta)
    {
        for (int i = 0; i < spawners.Count; ++i)
        {
            spawners[i].LogicUpdate(delta);
        }
    }

    void StopGame()
    {
        korokke.StopGame();
        foreach(Pug p in pugs)
        {
            p.StopGame();
        }
        foreach(Spawner sp in spawners)
        {
            sp.StopGame();
        }
    }

    void BuildLevel()
    {
        korokke = Instantiate<Korokke>(korokkePrefab);
        korokke.Init(this);

        Spawner[] sceneSpawners = FindObjectsOfType<Spawner>();
        spawners = new List<Spawner>(sceneSpawners);
        foreach(Spawner sp in spawners)
        {
            sp.Init(this);
        }

        entitiesToRemove = new List<IEntity>();

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

        foreach (Spawner sp in spawners)
        {
            sp.Cleanup();
            sp.Kill();
        }

        foreach (Pug p in pugs)
        {
            p.Cleanup();
            p.Kill();
        }
    }

    public BoidData GetKorokkeMotionData()
    {
        return korokke.data;
    }

    public void SpawnPug(Vector2 spawnPos)
    {
        Pug p = Instantiate<Pug>(pugPrefab);
        p.Init(this);
        Vector2 pos = spawnPos;
        float angle = Random.Range(0, 360);
        pos.Set(pos.x + Mathf.Cos(angle), pos.y + Mathf.Sin(angle));

        p.boidData.pos = pos;

        float speedPercent = maxSpeedPercentage * p.boidData.maxSpeed;
        p.boidData.maxSpeed += Random.Range(0.0f, speedPercent);

        p.StartGame();
        pugs.Add(p);
        NextPugID++;
    }

    public void ExhaustedSpawner(Spawner spawn)
    {
        // Replace with coroutine
        entitiesToRemove.Add(spawn);
    }

    public void PugTapped(Pug p)
    {
        // Replace with coroutine
        entitiesToRemove.Add(p);
    }
}
