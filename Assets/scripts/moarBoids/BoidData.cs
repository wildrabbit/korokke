using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoidData
{
    public Vector2 pos;
    public Vector2 acceleration;
    public Vector2 velocity;
    public Vector2 heading;
    public Vector2 side; // Perp. to heading

    public float mass;
    public float maxSpeed;
    public float maxForce;
    public float maxAngularSpeed;

    public Vector2 target;
}
