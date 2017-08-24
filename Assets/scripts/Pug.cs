using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pug : MonoBehaviour, IEntity
{
    SpriteRenderer spriteRendererRef;
    GameplayManager gameplayMgr;

    public float defaultViewRotation;
    public BoidData boidData;
    List<IBoidSteeringBehaviour> steerSequence;

    private void Awake()
    {
        spriteRendererRef = GetComponent<SpriteRenderer>();
    }
    public void Init(GameplayManager gpMgr)
    {
        gameplayMgr = gpMgr;
        transform.SetParent(gameplayMgr.sceneRoot);
        steerSequence = new List<IBoidSteeringBehaviour>();
    }

    public void StartGame()
    {
        // Set default boid values.
        transform.position = boidData.pos;
        boidData.target = gameplayMgr.GetKorokkePosition();
        // Register default steer sequence.
        steerSequence.Add(new SeekBehaviour());
    }

    public void LogicUpdate(float dt)
    {
        boidData.target = gameplayMgr.GetKorokkePosition();
        UpdateMotion(dt);
        if (Vector2.Distance(boidData.pos, boidData.target) < 0.1f)
        //if (Vector2.Distance(boidData.pos, boidData.target) > 10.0f)
        {
            boidData.velocity = Vector2.zero;
            steerSequence.Clear();
        }
    }

    void UpdateMotion(float dt)
    {
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
        Vector2 result = Vector2.zero;
        foreach(IBoidSteeringBehaviour behaviour in steerSequence)
        {
            // Compose?
            result += behaviour.Steer(dt, boidData);
        }
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
}
