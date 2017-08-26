using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Korokke : MonoBehaviour, IEntity
{
    [Header("Config")]
    public int maxKorokke = 3;

    public float itemX = 0.2f;
    public float itemY = 0.2f;

    public Transform korokkeItemPrefab;
    
    [HideInInspector]
    public int korokkeLeft;    
    List<Transform> korokkes;
    Transform plate;

    //SpriteRenderer spriteRendererRef;
    GameplayManager gameplayMgr;
    public Boid data;

    private void Awake()
    {
        //spriteRendererRef = GetComponent<SpriteRenderer>();
        plate = transform.Find("plate");
    }

    public void Init(GameplayManager gameplayMgrRef)
    {
        gameplayMgr = gameplayMgrRef;
        name = gameplayMgr.korokkePrefab.name;
        transform.SetParent(gameplayMgr.sceneRoot);
    }

    public void LoadData(KorokkeConfig config)
    {
        data.pos = gameplayMgr.korokkePosition;
        transform.position = data.pos;

        maxKorokke = config.maxKorokkes;
        korokkeLeft = maxKorokke;

        itemX = config.korokkeItemSpreadX;
        itemY = config.korokkeItemSpreadY;
    }

    // Use this for initialization
    public void StartGame()
    {
        korokkeLeft = maxKorokke;
        korokkes = new List<Transform>();
        for (int i = 0; i < korokkeLeft; ++i)
        {
            Transform krok = Instantiate<Transform>(korokkeItemPrefab);
            krok.SetParent(plate);
            Vector2 pos = krok.position;
            pos.x += Random.Range(-itemX, itemX);
            pos.y += Random.Range(-itemY, itemY);
            krok.position = pos;
            krok.rotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward);
            korokkes.Add(krok);
        }
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
        int idx = korokkes.Count - 1;
        Transform krok = korokkes[idx];
        korokkes.RemoveAt(idx);
        Destroy(krok.gameObject);
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
