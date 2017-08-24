using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Korokke : MonoBehaviour, IEntity
{
    [Header("Config")]
    public int maxKorokke = 3;
    
    int korokke;

    SpriteRenderer spriteRendererRef;
    GameplayManager gameplayMgr;

    private void Awake()
    {
        spriteRendererRef = GetComponent<SpriteRenderer>();
    }

    public void Init(GameplayManager gameplayMgrRef)
    {
        gameplayMgr = gameplayMgrRef;
        transform.position = gameplayMgr.korokkePosition;
    }

    // Use this for initialization
    public void StartGame()
    {
        korokke = maxKorokke;
    }

    public void StopGame()
    {

    }

    public void LogicUpdate(float dt)
    {
        if (korokke == 0) return;
    }
	
    public void Hit(int damage = 1)
    {
        korokke = (damage > korokke) ? 0 : damage - korokke;
        if (korokke == 0)
        {
            // Death!!
            gameObject.SetActive(false);
        }
    }

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
