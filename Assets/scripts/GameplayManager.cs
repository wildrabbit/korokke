using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

#region config
    [Header("Level management")]
    public string defaultScene;
    string currentScene;
    string nextScene;
    [HideInInspector]
    public int levelIdx;
    [HideInInspector]
    public string levelTitle;
    [HideInInspector]
    public string levelDesc;
    
    [Header("Scene config")]
    public Transform sceneRoot;
    public Transform pugRoot;
    public Vector3 korokkePosition;

    [Header("Gameplay vars")]
    public float korokkeDistanceThreshold = 0.25f;
    public float korokkeEscapeThreshold = 10.0f;

    float spawnerRadius = 1.0f;
    float maxSpeedPercentage = 2.0f;

    //float boidJitter = 0.1f;
    //float boidDistance = 2.0f;
    //float boidRadius = 1.0f;
    //The heading is the target.

    [Header("Prefabs")]
    public Korokke korokkePrefab;
    public Pug pugPrefab;

    [HideInInspector]
    public int pugsLeft = 0;
    [HideInInspector]
    public int pendingSpawners = 0;
#endregion

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
    [HideInInspector]
    public Action OnGameOver;
    [HideInInspector]
    public Action OnLevelExit;

    bool loading = false;

    // Use this for initialization
    void Start ()
    {
        currentScene = defaultScene;
        LoadLevel(currentScene);
	}

    void StartGame()
    {
        foreach(IEntity ent in entities)
        {
            ent.StartGame();
        }
        escapees = 0;
        if (OnGameStarted != null)
        {
            OnGameStarted(pugsLeft, korokke.korokkeLeft);
        }
    }

    public void LevelIntroFinished()
    {
        running = true;
    }

    // Update is called once per frame
    void Update ()
    {
        if (loading) return;
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
        StopGame();
        Debug.Log("WON");
        yield return new WaitForSeconds(1.0f);
        if (OnGameWon != null)
        {
            OnGameWon();
        }
    }

    IEnumerator GameLost()
    {
        StopGame();
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

    void LoadLevel(string name)
    {
        loading = true;

        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        StartCoroutine(LoadLevelAsync(name));
    }

    IEnumerator LoadLevelAsync(string name)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);

        while (!asyncOp.isDone)
        {
            yield return null;
        }

        BuildLevel();
        StartGame();
        loading = false;
        yield return null;
    }

    public void Run()
    {
        running = true;
    }

    void BuildLevel()
    {
        running = false;

        entities = new List<IEntity>();
        entitiesToAdd = new List<IEntity>();
        entitiesToRemove = new List<IEntity>();
        
        LevelConfig cfg = FindObjectOfType<LevelConfig>();
        if (cfg == null)
        {
            return;
        }
        nextScene = cfg.nextScene;
        maxSpeedPercentage = cfg.maxSpeedNoisePercentage;
        levelIdx = cfg.levelIdx;
        levelTitle = cfg.levelName;
        levelDesc = cfg.levelDesc;

        korokkeDistanceThreshold = cfg.korokkeStealDistanceThreshold;
        korokkeEscapeThreshold = cfg.korokkeEscapeDistanceThreshold;
        spawnerRadius = cfg.spawnerSpreadRadius;

        DestroyImmediate(cfg.gameObject);

        pugsLeft = 0;
        pendingSpawners = 0;

        BuildGoal();
        BuildSpawners();
        pugs = new List<Pug>();

        
    }

    void BuildGoal()
    {
        KorokkeConfig korokkeCfg = FindObjectOfType<KorokkeConfig>();
        
        Korokke korokkeInstance = Instantiate<Korokke>(korokkePrefab);
        korokkeInstance.Init(this);
        korokkeInstance.LoadData(korokkeCfg);
        AddEntity(korokkeInstance);

        DestroyImmediate(korokkeCfg.gameObject);
    }

    void BuildSpawners()
    {
        GameObject points = GameObject.Find("spawnPoints");
        if (points != null)
        {
            points.transform.SetParent(sceneRoot);
        }
        
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
        if (entities != null)
        {
            foreach (IEntity ent in entities)
            {
                ent.Cleanup();
                ent.Kill();
            }
        }
        entities.Clear();
        entities = null;
        korokke = null;
        if (pugs != null)
        {
            pugs.Clear();
            pugs = null;
        }

        if (spawners != null)
        {
            spawners.Clear();
            spawners = null;
        }

        if (OnLevelExit != null)
        {
            OnLevelExit();
        }
    }

    public Boid GetKorokkeMotionData()
    {
        return korokke.data;
    }

    public void SpawnPug(Vector2 spawnPos)
    {
        Pug p = Instantiate<Pug>(pugPrefab);
        p.Init(this);

        Vector2 pos = spawnPos;
        float angle = URandom.Range(0, 360);
        pos.Set(pos.x + spawnerRadius * Mathf.Cos(angle), pos.y + spawnerRadius * Mathf.Sin(angle));
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

    public void NextLevel()
    {
        ExitLevel();
        if (!string.IsNullOrEmpty(nextScene))
        {
            currentScene = nextScene;
            LoadLevel(currentScene);
        }    
        else
        {
            if (OnGameOver != null)
            {
                OnGameOver();
            }
        }
    }

    public void RepeatLevel()
    {
        ExitLevel();
        LoadLevel(currentScene);
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }
}

