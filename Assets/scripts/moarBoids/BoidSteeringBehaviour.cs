using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoidSteeringBehaviour
{
    Vector2 Steer(float dt, BoidData data);
}

public class SeekBehaviour: IBoidSteeringBehaviour
{
    public Vector2 Steer(float dt, BoidData data)
    {
        Vector2 targetVec = (data.target - data.pos).normalized * data.maxSpeed;
        return targetVec - data.velocity;
    }
}

public class FleeBehaviour : IBoidSteeringBehaviour
{
    public Vector2 Steer(float dt, BoidData data)
    {
        if (Vector2.Distance(data.pos, data.target) > 0.0f)
        {
            Vector2 targetVec = (data.pos - data.target).normalized * data.maxSpeed;
            return targetVec - data.velocity;
        }
        else
        {
            float angle = Random.Range(0, Mathf.PI * 2);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }
}

