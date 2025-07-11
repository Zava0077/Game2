using AStar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.XR;
using static AStar.BackTrackingAStar;
public abstract class Enemy : Entity 
{
    public int offset;
    public Aggressiveness Agro { get; private set; }
    public Entity Target => Agro.Target;
    public override EntityType TypeOfEntity { get; protected set; }
    protected Type[] toAvoid = new Type[] { typeof(Rat), typeof(Skeleton), typeof(Spider), typeof(ChestMimic), typeof(Chest) };
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
        Agro = (100, Player.character);
    }
    public override void Interact(Entity whoInteracts)
    {
        base.Interact(whoInteracts);
        Agro.UpdateValue(whoInteracts);
    } 
    public List<Vector2Int> GetPath(Entity target) =>
        _pathfinder.GetPath(this, target, AvoidEverything, 8, toAvoid).Skip(1).Select(x => new Vector2Int(x.X, x.Y)).ToList();
}
