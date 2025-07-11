using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static SquareCreator;
using static AnimationHelper;
using static AStar.BackTrackingAStar;
using AStar;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections.Generic;
public abstract class EntityType 
{
    protected Entity _owner;
    public EntityType(Entity owner) => _owner = owner;
    public abstract void Materialize();
}
public class AbstractEntity : EntityType
{
    public AbstractEntity(Entity owner) : base(owner)
    {
    }

    public override void Materialize()
    {
        return;
    }
}
public class ConcreteEntity : EntityType
{
    public ConcreteEntity(Entity owner) : base(owner)
    {
    }

    public override void Materialize()
    {
        map[_owner.GridPosition.x, _owner.GridPosition.y].currentEntity = _owner;
    }
}
public abstract class Entity : MonoBehaviour //создать класс с интеллектом
{
    public const float ANIM_DURATION = 0.15f;
    public static readonly List<Entity> entities = new();
    public readonly System.Random rnd = new System.Random();
    public readonly Vector2[] directions = new Vector2[] { Vector2.down, Vector2.left, Vector2.right, Vector2.up };
    public EntityStats stats = new(); 
    public readonly BackTrackingAStar _pathfinder = new()
    {
        DistanceCounter = CountDistanceDiagonally,
    }; 
    public Vector2Int GridPosition = Vector2Int.zero;
    protected Sprite c_Sprite;
    public abstract EntityType TypeOfEntity { get; protected set; }
    public abstract string Name { get; }
    public virtual RenderLevels? RenderLevel { get; internal set; } = RenderLevels.Entities;
    protected MovementService MovementService { get; private set; }
    public Vector2Int Direction => MovementService.EntityDirection;
    public Vector3 NewPos { get; set; }
    public Sprite Sprite => GetSprite();
    protected void Awake()
    {
        entities.Add(this);
        MovementService = new MovementService(this);
        if (this is ILightning lighter)
            Player.OnTimeStep += () => lighter.LightUp();
    }
    protected void Start()
    {
        TypeOfEntity.Materialize();
        transform.position = new(transform.position.x, transform.position.y, (float)RenderLevel);
        NewPos = transform.position;
        if (this is IFogRevealer revealer)
            Fog.UpdateFogOfWar(revealer.RevealRadius, this, Fog.Smooth);
    }
    public virtual void Interact(Entity whoInteracts)
    {
        if (whoInteracts is Enemy ew) 
            ew.Agro.UpdateValue(null);
        LogManager.Log($"Взаимодействие: {whoInteracts} с {this}" + (whoInteracts is Enemy e ? $" Агрессия к {e.Agro.Target }: {e.Agro.Value}" : string.Empty));
        StartCoroutine(InteractAnimation(whoInteracts));
    }
    private IEnumerator InteractAnimation(Entity partner) 
    {
        if (partner == null) yield break; //мб баг будет
        Transform partnerTransform = partner.transform;
        Vector3 originalPosition = partnerTransform.position;
        Vector3 direction = (NewPos - originalPosition).normalized;
        Vector3 targetPosition = originalPosition + direction * 0.2f; 
        float duration = 0.1f;
        yield return LerpAnim(partner.gameObject, targetPosition, duration, x => x, partnerTransform.position.z);
        partnerTransform.position = targetPosition;
        yield return LerpAnim(partner.gameObject, originalPosition, duration, x => x, partnerTransform.position.z);
        partnerTransform.position = originalPosition;
    }
    /// <summary>
    /// Направляет объект по клеточному полю, в сторону точки
    /// </summary>
    /// <param name="toWhere"></param>
    public void MoveTowards(Vector2 toWhere)
    {
        if (toWhere != Vector2.negativeInfinity)
        {
            Vector2Int targetPos = GridPosition + (toWhere - (Vector2)GridPosition).normalized.MaxContrastInt();
            MovementService.TryDisplace(targetPos);
            
            if (this is IFogRevealer revealer) Fog.UpdateFogOfWar(revealer.RevealRadius, this, Fog.Smooth);
        }
    }
    public void MoveTowards(Entity target) => 
        MoveTowards(target.GridPosition);
    private Sprite GetSprite()
    {
        if (!c_Sprite) c_Sprite = LoadSprite();
        return c_Sprite;
    }
    public override string ToString() => Name;
    protected abstract Sprite LoadSprite();
}
