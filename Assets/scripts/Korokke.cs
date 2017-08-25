using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Korokke : MonoBehaviour, IEntity
{
    [Header("Config")]
    public int maxKorokke = 3;
    
    public int korokkeLeft;

    //SpriteRenderer spriteRendererRef;
    GameplayManager gameplayMgr;
    public BoidData data;

    private void Awake()
    {
        //spriteRendererRef = GetComponent<SpriteRenderer>();
    }

    public void Init(GameplayManager gameplayMgrRef)
    {
        gameplayMgr = gameplayMgrRef;
        name = gameplayMgr.korokkePrefab.name;
        transform.SetParent(gameplayMgr.sceneRoot);
        data.pos = gameplayMgr.korokkePosition;
        transform.position = data.pos;
    }

    // Use this for initialization
    public void StartGame()
    {
        korokkeLeft = maxKorokke;
    }

    public void StopGame()
    {

    }

    public void LogicUpdate(float dt)
    {
        if (korokkeLeft == 0) return;
        // Just to be consistent: 
        transform.position = data.pos;
    }
	
    public void Hit(int damage = 1)
    {
        Debug.Log("Eating Korokke!");
        korokkeLeft = (damage > korokkeLeft) ? 0 : korokkeLeft - damage;
        Debug.Log($"Left: {korokkeLeft}");
    }

    public void Cleanup()
    {
        //spriteRendererRef = null;
        gameplayMgr = null;
    }

    public void Kill(float delay = 0.0f)
    {
        Destroy(gameObject, delay);
    }

    public void OnEntityAdded()
    {
        gameplayMgr.SetKorokke(this);
    }
    public void OnEntityRemoved()
    {
        gameplayMgr.SetKorokke(null);
    }
}
