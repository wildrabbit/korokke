using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

enum PugState
{
    SeekKorokke = 0,
    Escape
}

public class Pug : MonoBehaviour, IEntity
{
    delegate Vector2 SteerFunction(Boid d);

    //SpriteRenderer spriteRendererRef;
    GameplayManager gameplayMgr;
    TapGesture tapGesture;

    public float defaultViewRotation;
    public float escapeSpeedIncrease = 1.5f;
    public float escapeForceIncrease = 1.2f;
    public Boid boidData;

    PugState state;

    public bool Escaping
    {
        get { return state == PugState.Escape; }
    }


    private void Awake()
    {
        //spriteRendererRef = GetComponent<SpriteRenderer>();
        tapGesture = GetComponent<TapGesture>();
    }
    public void Init(GameplayManager gpMgr)
    {
        gameplayMgr = gpMgr;
        transform.SetParent(gameplayMgr.pugRoot);
        name = string.Format("pug_{0:00}", GameplayManager.NextPugID);
    }

    public void StartGame()
    {
        // Set default boid values.
        transform.position = boidData.pos;
        boidData.target = gameplayMgr.GetKorokkeMotionData();
        tapGesture.Tapped += OnTap;
        state = PugState.SeekKorokke;
    }

    public void OnTap(object o, EventArgs args)
    {
        // Check type, whatever...
        gameplayMgr.PugTapped(this);
    }

    public void LogicUpdate(float dt)
    {        
        boidData.target = gameplayMgr.GetKorokkeMotionData();
        UpdateMotion(dt);
    }

    void UpdateMotion(float dt)
    {
        Vector2 steerForce = CalculateTotalSteering(dt);

        switch (state)
        {
            case PugState.SeekKorokke:
                {
                    if (boidData.target == null || Vector2.Distance(boidData.pos, boidData.target.pos) < gameplayMgr.korokkeDistanceThreshold)
                    {
                        return;
                    }
                    break;
                }
            case PugState.Escape:
                {
                    if (boidData.target != null && Vector2.Distance(boidData.pos, boidData.target.pos) > gameplayMgr.korokkeEscapeThreshold)
                    {
                        gameplayMgr.PugEscaped(this);
                    }
                    break;
                }
        }
        
        boidData.acceleration = steerForce / boidData.mass;
        boidData.velocity += boidData.acceleration * dt;
        Vector2.ClampMagnitude(boidData.velocity, boidData.maxSpeed);
        boidData.pos += (boidData.velocity * dt);
        transform.position = boidData.pos;

        if (boidData.velocity.magnitude > Mathf.Epsilon)
        {
            boidData.heading = boidData.velocity.normalized;
            boidData.side = new Vector2(boidData.velocity.y, -boidData.velocity.x).normalized;
            float angle = Vector2.SignedAngle(boidData.heading, Vector2.right) + defaultViewRotation;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
        }

        // Check if wrap??
    }

    Vector2 CalculateTotalSteering(float dt)
    {
        SteerFunction[] functions = new SteerFunction[] { BoidSteeringFunctions.Arrive, BoidSteeringFunctions.Flee};
        Vector2 result = functions[(int)state](boidData); // BoidSteeringFunctions.ApplyJitter(boidData, functions[(int)state](boidData), gameplayMgr.boidJitter, gameplayMgr.boidRadius, gameplayMgr.boidDistance);
        return result;
    }

    public void StopGame()
    {

    }

    public void StoleKorokke()
    {
        boidData.maxSpeed *= escapeSpeedIncrease;
        boidData.maxForce *= escapeForceIncrease;
        tapGesture.Tapped -= OnTap;

        state = PugState.Escape;
    }

    // Use this for initialization
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
        gameplayMgr.AddPug(this);
    }
    public void OnEntityRemoved()
    {
        gameplayMgr.RemovePug(this);
    }
}
