using System;
using System.Collections;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

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
        SquareCreator.map[_owner.GridPosition.x, _owner.GridPosition.y].currentEntity = _owner;
    }
}

public abstract class Entity : MonoBehaviour
{
    public const float ANIM_DURATION = 0.15f;
    public readonly System.Random rnd = new System.Random();
    public readonly EntityStats stats = new();
    public Vector2Int GridPosition = Vector2Int.zero;
    protected Sprite c_Sprite;
    public abstract EntityType TypeOfEntity { get; protected set; }
    protected MovementService MovementService { get; private set; }
    public Vector2Int Direction => MovementService.EntityDirection;
    public Vector3 NewPos { get; set; }
    public Sprite Sprite => GetSprite();
    protected void Awake()
    {
        MovementService = new MovementService(this);
    }
    protected void Start()
    {
        TypeOfEntity.Materialize();
        NewPos = transform.position;
    }
    public virtual void Interact(Entity whoInteracts)
    {
        LogManager.Log($"¬заимодействие:{whoInteracts} с {this}");
        StartCoroutine(InteractAnimation(whoInteracts));
    }
    private IEnumerator InteractAnimation(Entity partner) //чуть переработать и потом добавить метод на тр€ску персонажа с которым взаимодействуют
    {
        if (partner == null) yield break;
        Transform partnerTransform = partner.transform;
        Vector3 originalPosition = partnerTransform.position;
        Vector3 direction = (NewPos - originalPosition).normalized;
        Vector3 targetPosition = originalPosition + direction * 0.2f; 
        float duration = 0.1f;
        float time = 0f;
        while (time < duration)
        {
            partnerTransform.position = Vector3.Lerp(originalPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        partnerTransform.position = targetPosition;
        time = 0f;
        while (time < duration)
        {
            partnerTransform.position = Vector3.Lerp(targetPosition, originalPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        partnerTransform.position = originalPosition;
    }
    /// <summary>
    /// Ќаправл€ет объект по клеточному полю, в сторону точки
    /// </summary>
    /// <param name="toWhere"></param>
    protected void MoveTowards(Vector2 toWhere)
    {
        if (toWhere != Vector2.negativeInfinity)
        {
            Vector2Int targetPos = GridPosition + (toWhere - (Vector2)GridPosition).normalized.MaxContrastInt();
            MovementService.TryDisplace(targetPos);
        }
    }
    public void MoveTowards(Entity target) => 
        MoveTowards(target.GridPosition);
    private Sprite GetSprite()
    {
        if (!c_Sprite) c_Sprite = LoadSprite();
        return c_Sprite;
    }
    protected abstract Sprite LoadSprite();
}
