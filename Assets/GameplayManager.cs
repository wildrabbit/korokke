using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Config")]
    public Transform sceneRoot;
    public Vector3 korokkePosition;

    [Header("Prefabs")]
    public Korokke korokkePrefab;

    //------------------------
    Korokke korokke;
	// Use this for initialization
	void Start ()
    {
        BuildLevel();	
	}
	
	// Update is called once per frame
	void Update () {
        float delta = Time.deltaTime;
        korokke.LogicUpdate(delta);
	}

    void BuildLevel()
    {
        korokke = Instantiate<Korokke>(korokkePrefab);
        korokke.Init(this);
    }


    void ExitLevel()
    {
        korokke.Cleanup();
        korokke.Kill();
    }
}
