using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelConfig : MonoBehaviour
{
    public string nextScene;
    public float korokkeStealDistanceThreshold = 0.8f;
    public float korokkeEscapeDistanceThreshold = 10.0f;

    public float spawnerSpreadRadius = 2.0f;

    public float maxSpeedNoisePercentage = 0.15f;

    public string levelName;
    public string levelDesc;
    public int levelIdx;

    //[Header("Boid jitter config")]
    //public float boidJitter = 4.0f;
    //public float boidJitterRadius = 3.0f;
    //public float boidJitterDistance = 2.0f;
}
