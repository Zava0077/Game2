using System;
using UnityEngine;

public abstract class Enemy : Entity
{
    protected readonly Vector2[] directions = new Vector2[] { Vector2.down, Vector2.up, Vector2.left, Vector2.right };
    protected int offset;
    public abstract void OnMove();
    protected void Start()
    {
        offset = rnd.Next(0, 4);
        Player.OnTimeStep += OnMove;
    }
     
}
