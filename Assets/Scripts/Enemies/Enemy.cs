using AStar;
using System;
using UnityEngine;

public abstract class Enemy : Entity
{
    protected readonly Vector2[] directions = new Vector2[] { Vector2.down, Vector2.up, Vector2.left, Vector2.right };
    protected int offset;
    protected BackTrackingAStar _pathfinder = new();
    public override EntityType TypeOfEntity { get; protected set; }
    public abstract void OnMove();
    protected new void Awake()
    {
        base.Awake();
        TypeOfEntity = new ConcreteEntity(this);
    }
    protected new void Start()
    {
        base.Start();
        offset = rnd.Next(0, 4);
        Player.OnTimeStep += OnMove;
    }
}
