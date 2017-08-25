using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, IEntity
{
    public int pugCount = 1;
    public int totalPugs = 1;
    public float totalTime = 1.0f;
    public float spawnNoise = 0.2f;
    public float minDelay = 0.1f;

    GameplayManager gameplayManager;

    int pugsLeft;
    float elapsed;
    float currentDelay;

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
        pugsLeft = pugCount;
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
        if (pugCount == 0) return;

        elapsed += dt;
        if (elapsed < currentDelay) return;
        
        gameplayManager.SpawnPug(transform.position);
        ResetSpawnCounter();
        pugCount--;
        if (pugCount == 0)
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
}
