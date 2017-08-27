using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, IEntity
{
    public int totalPugs = 1;
    public float totalTime = 1.0f;
    public float spawnNoise = 0.2f;
    public float minDelay = 0.1f;
    public float startDelay = 0.0f;

    GameplayManager gameplayManager;

    bool started = false;
    int pugsLeft;
    float elapsed;
    float currentDelay;
    float startElapsed;

    public bool Depleted
    {
        get { return pugsLeft == 0; }
    }

    public void Init(GameplayManager mgr)
    {
        gameplayManager = mgr;
    }

    public void StartGame()
    {
        ResetSpawnCounter();
        pugsLeft = totalPugs;

        gameplayManager.pendingSpawners++;
        gameplayManager.pugsLeft += pugsLeft;
        bool hasDelay = startDelay > 0.0f;
        started = !hasDelay;
        if (hasDelay)
        {
            startElapsed = 0.0f;            
        }
    }

    void ResetSpawnCounter()
    {
        currentDelay = totalTime / (float)totalPugs;
        currentDelay += Random.Range(-spawnNoise, spawnNoise);
        currentDelay = Mathf.Max(currentDelay, minDelay);
        elapsed = 0.0f;
    }

    public void LogicUpdate(float dt)
    {
        if (!started)
        {
            startElapsed += dt;
            if (startElapsed > startDelay)
            {
                started = true;
            }
            else return;
        }

        if (pugsLeft == 0) return;

        elapsed += dt;
        if (elapsed < currentDelay) return;
        
        gameplayManager.SpawnPug(transform.position);
        ResetSpawnCounter();
        pugsLeft--;
        if (pugsLeft == 0)
        {
            // Do something to the spawner
            gameplayManager.ExhaustedSpawner(this);
        }
    }

    public void StopGame()
    {

    }

    public void Cleanup()
    {

    }

    public void Kill(float delay = 0.0f)
    {
        Destroy(gameObject, delay);
    }

    public void OnEntityAdded()
    {
        gameplayManager.AddSpawner(this);
    }
    public void OnEntityRemoved()
    {
        gameplayManager.RemoveSpawner(this);
    }
}
