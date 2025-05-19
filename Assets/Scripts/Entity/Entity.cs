using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public readonly System.Random rnd = new System.Random();
    public readonly EntityStats stats = new();
    protected Sprite c_Sprite;
    public Vector2Int GridPosition = Vector2Int.zero;
    protected MovementService MovementService { get; private set; }
    public Vector2Int Direction => MovementService.EntityDirection;
    public Sprite Sprite => GetSprite();
    protected void Awake() =>
        MovementService = new MovementService(this);
    public virtual void Interact(Entity whoInteracts)
    {
        LogManager.Log($"Взаимодействие:{whoInteracts} с {this}");
    }
    /// <summary>
    /// Направляет объект по клеточному полю, в сторону точки
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
    protected void MoveTowards(Entity target) => 
        MoveTowards(target.GridPosition);
    private Sprite GetSprite()
    {
        if (!c_Sprite) c_Sprite = LoadSprite();
        return c_Sprite;
    }
    protected abstract Sprite LoadSprite();
}
