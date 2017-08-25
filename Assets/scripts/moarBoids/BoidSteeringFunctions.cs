using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoidSteeringFunctions
{
    public static Vector2 Seek(float dt, BoidData data)
    {
        Vector2 targetVec = (data.target.pos - data.pos).normalized * data.maxSpeed;
        return targetVec - data.velocity;
    }

    public static Vector2 Flee(BoidData data)
    {
        if (Vector2.Distance(data.pos, data.target.pos) > 0.0f)
        {
            Vector2 targetVec = (data.pos - data.target.pos).normalized * data.maxSpeed;
            return targetVec - data.velocity;
        }
        else
        {
            float angle = Random.Range(0, Mathf.PI * 2);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }

    public static Vector2 Arrive(BoidData data)
    {
        Vector2 toTarget = data.target.pos - data.pos;
        float distance = toTarget.magnitude;

        if (distance > 0.0f)
        {
            float speed = distance / ((float)data.decelStrength * (int)data.decelType);
            speed = Mathf.Min(speed, data.maxSpeed);

            Vector2 desired = toTarget * speed / distance;
            return desired - data.velocity;
        }
        return Vector2.zero;
    }

    public static Vector2 Wander(BoidData data)
    {
        // Start by displacing the target
        data.wanderTarget += new Vector2(Random.Range(-1.0f, 1.0f) * data.wanderJitter, Random.Range(-1.0f, 1.0f) * data.wanderJitter);
        data.wanderTarget.Normalize();
        // Project target back to the centered circle
        data.wanderTarget *= data.wanderRadius;
        // And now cast the circle in front of the boid
        float posLen = data.pos.magnitude;
        Vector2 projectedTarget = data.pos.normalized * (posLen + data.wanderDistance) + data.wanderTarget;
        return projectedTarget - data.pos;

    }

    public static Vector2 ApplyJitter(BoidData data, Vector2 force, float jitter, float radius, float distance)
    {
        Vector2 forceN = force.normalized;
        forceN += new Vector2(Random.Range(-1.0f, 1.0f) * data.wanderJitter, Random.Range(-1.0f, 1.0f) * data.wanderJitter);
        forceN.Normalize();

        forceN *= radius;

        Vector2 projected = force.normalized * (force.magnitude + distance) + forceN;
        
        return projected - force;
    }
}