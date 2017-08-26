using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoidDecelerationType
{
    Slow = 3,
    Normal = 2,
    Fast = 1
}

[Serializable]
public class Boid
{
    public Vector2 pos;
    public Vector2 acceleration;
    public Vector2 velocity;
    public Vector2 heading;
    public Vector2 side; // Perp. to heading

    public BoidDecelerationType decelType = BoidDecelerationType.Normal;
    public float decelStrength = 0.3f;

    public float mass;
    public float maxSpeed;
    public float maxForce;
    public float maxAngularSpeed;

    public Boid target;

    public float wanderRadius;
    public float wanderDistance; // Distance the circle is projected in front of the agent!
    public float wanderJitter; // Max. amount of random displacement
    public Vector2 wanderTarget;
}
