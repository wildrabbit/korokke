using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

public class Pug : MonoBehaviour, IEntity
{
    SpriteRenderer spriteRendererRef;
    GameplayManager gameplayMgr;
    TapGesture tapGesture;

    public float defaultViewRotation;
    public BoidData boidData;



    private void Awake()
    {
        spriteRendererRef = GetComponent<SpriteRenderer>();
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
        if (boidData.target == null || Vector2.Distance(boidData.pos, boidData.target.pos) < gameplayMgr.korokkeDistanceThreshold)
        {
            boidData.velocity = Vector2.zero;
            return;
        }

        Vector2 steerForce = CalculateTotalSteering(dt);
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
        Vector2 result = BoidSteeringFunctions.ApplyJitter(boidData, BoidSteeringFunctions.Arrive(boidData), gameplayMgr.boidJitter, gameplayMgr.boidRadius, gameplayMgr.boidDistance);
        return result;
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

    public void OnEntityAdded()
    {
        gameplayMgr.AddPug(this);
    }
    public void OnEntityRemoved()
    {
        gameplayMgr.RemovePug(this);
    }
}
