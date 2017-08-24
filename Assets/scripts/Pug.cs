using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pug : MonoBehaviour, IEntity
{
    SpriteRenderer spriteRendererRef;
    GameplayManager gameplayMgr;

    public void Init(GameplayManager gpMgr)
    {

    }

    public void StartGame()
    {

    }

    public void LogicUpdate(float dt)
    {

    }

    public void StopGame()
    {

    }

    // Use this for initialization
    public void Cleanup()
    {
        spriteRendererRef = null;
        gameplayMgr = null;
    }

    public void Kill(float delay = 0.0f)
    {
        Destroy(gameObject, delay);
    }
}
