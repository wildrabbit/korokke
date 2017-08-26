using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using URandom = UnityEngine.Random;

public enum LevelResult
{
    None = 0,
    Won,
    Lost
}
public class GameplayManager : MonoBehaviour
{
    public static int NextPugID = 0;

    [Header("Config")]
    public Transform sceneRoot;
    public Transform pugRoot;
    public Vector3 korokkePosition;

    public float korokkeDistanceThreshold = 0.25f;
    public float korokkeEscapeThreshold = 10.0f;

    public float spawnerRadius = 0.15f;
    public float maxSpeedPercentage = 2.0f;

    public float boidJitter = 0.1f;
    public float boidDistance = 2.0f;
    public float boidRadius = 1.0f;
    //The heading is the target.

    public int pugsLeft = 0;
    public int pendingSpawners = 0;
    
    [Header("Prefabs")]
    public Korokke korokkePrefab;
    public Pug pugPrefab;

    List<IEntity> entities;
    Korokke korokke;
    List<Pug> pugs;
    List<Spawner> spawners;

    List<IEntity> entitiesToRemove;
    List<IEntity> entitiesToAdd;

    bool running = false;
    int escapees = 0;

    [HideInInspector]
    public Action<int> OnKorokkeStolen;
    [HideInInspector]
    public Action<int> OnPugHit;
    [HideInInspector]
    public Action<int, int> OnGameStarted;
    [HideInInspector]
    public Action OnGameWon;
    [HideInInspector]
    public Action OnGameLost;

    // Use this for initialization
    void Start ()
    {
        BuildLevel();
        StartGame();
	}

    void StartGame()
    {
        foreach(IEntity ent in entities)
        {
            ent.StartGame();
        }
        escapees = 0;
        running = true;
        if (OnGameStarted != null)
        {
            OnGameStarted(pugsLeft, korokke.korokkeLeft);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!running) return;

        float delta = Time.deltaTime;
        foreach(IEntity ent in entities)
        {
            ent.LogicUpdate(delta);
        }

        int eatenCroquettes = 0;
        foreach (Pug p in pugs)
        {
            if (korokke.korokkeLeft == 0 || p.Escaping) continue;

            if (Vector2.Distance(p.boidData.pos, korokke.data.pos) < korokkeDistanceThreshold)
            {
                korokke.Hit();
                p.StoleKorokke();
                escapees++;
                eatenCroquettes++;
                if (OnKorokkeStolen != null)
                {
                    OnKorokkeStolen(korokke.korokkeLeft);
                }
            }
        }

        for (int i = 0; i < entitiesToRemove.Count; ++i)
        {
            RemoveEntity(entitiesToRemove[i]);          
        }
        entitiesToRemove.Clear();

        for (int i = 0; i < entitiesToAdd.Count; ++i)
        {
            AddEntityStart(entitiesToAdd[i]);
        }
        entitiesToAdd.Clear();

        LevelResult result = CalculateResults();
        if (result == LevelResult.None) return;
        if (result == LevelResult.Won)
        {
            StartCoroutine(GameWon());
        }
        else
        {
            StartCoroutine(GameLost());
        }
        StopGame();

    }

    void AddEntity(IEntity e)
    {
        entities.Add(e);
        e.OnEntityAdded();
    }

    void AddEntityStart(IEntity e)
    {
        e.StartGame();
        AddEntity(e);
    }

    void RemoveEntity(IEntity e)
    {
        // Clean this, or add several lists
        e.OnEntityRemoved();
        entities.Remove(e);
        e.Kill();
    }

    IEnumerator GameWon()
    {
        Debug.Log("WON");
        yield return new WaitForSeconds(1.0f);
        if (OnGameWon != null)
        {
            OnGameWon();
        }
    }

    IEnumerator GameLost()
    {
        Debug.Log("LOST");
        yield return new WaitForSeconds(1.0f);
        if (OnGameLost != null)
        {
            OnGameLost();
        }
    }

    void StopGame()
    {
        running = false;
        foreach(IEntity ent in entities)
        {
            ent.StopGame();
        }
    }

    void BuildLevel()
    {
        running = false;
        entities = new List<IEntity>();
        BuildGoal();
        BuildSpawners();
        pugs = new List<Pug>();
        entitiesToAdd = new List<IEntity>();
        entitiesToRemove = new List<IEntity>();
    }

    void BuildGoal()
    {
        Korokke korokkeInstance = Instantiate<Korokke>(korokkePrefab);
        korokkeInstance.Init(this);
        AddEntity(korokkeInstance);
    }

    void BuildSpawners()
    {
        // TODO: Eventually instantiate, rather than find.
        spawners = new List<Spawner>();
        Spawner[] sceneSpawners = FindObjectsOfType<Spawner>();
        foreach (Spawner sp in sceneSpawners)
        {
            sp.Init(this);
            AddEntity(sp);
        }
    }

    void ExitLevel()
    {
        foreach(IEntity ent in entities)
        {
            ent.Cleanup();
            ent.Kill();
        }
        entities.Clear();
        entities = null;
        korokke = null;
        pugs.Clear();
        pugs = null;
        spawners.Clear();
        spawners = null;
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
        float angle = URandom.Range(0, 360);
        pos.Set(pos.x + Mathf.Cos(angle), pos.y + Mathf.Sin(angle));
        p.boidData.pos = pos;

        float speedPercent = maxSpeedPercentage * p.boidData.maxSpeed;
        p.boidData.maxSpeed += URandom.Range(0.0f, speedPercent);

        entitiesToAdd.Add(p);
        NextPugID++;
    }

    public void ExhaustedSpawner(Spawner spawn)
    {
        pendingSpawners--;
        entitiesToRemove.Add(spawn);
    }

    public void PugTapped(Pug p)
    {
        // Replace with coroutine
        pugsLeft--;
        entitiesToRemove.Add(p);
        if (OnPugHit != null)
        {
            OnPugHit(pugsLeft);
        }
    }

    public void PugEscaped(Pug p)
    {
        escapees--;
        entitiesToRemove.Add(p);
    }

    public LevelResult CalculateResults()
    {
        if (korokke.korokkeLeft == 0 && escapees == 0)
        {
            return LevelResult.Lost;
        }
        // All spawners depleted
        if (pendingSpawners == 0 && pugsLeft == 0)
        {
            return LevelResult.Won;
        }
        return LevelResult.None;
    }


    // Specific lists/members addition/removal methods
    public void SetKorokke(Korokke theKorokke)
    {
        korokke = theKorokke;
    }

    public void AddPug(Pug p)
    {
        pugs.Add(p);
    }
    public void AddSpawner(Spawner s)
    {
        spawners.Add(s);
    }

    public void RemovePug(Pug p)
    {
        pugs.Remove(p);
    }

    public void RemoveSpawner(Spawner sp)
    {
        spawners.Remove(sp);
    }
}

