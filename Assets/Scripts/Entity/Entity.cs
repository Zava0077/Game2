using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public readonly System.Random rnd = new System.Random();
    public readonly EntityStats stats = new();
    public Vector2Int GridPosition = Vector2Int.zero; 
    protected const float MOVE_DISPLACEMENT = 0.8f;
    public virtual void Interact(Entity whoInteracts)
    {
        Debug.Log($"Interact:{whoInteracts} with {this}");
    }
    protected static Vector2 MaxContrast(Vector2 vec) 
    {
        if (vec.x == vec.y) return vec.x < 0 ? Vector2.down : Vector2.up; 
        if (Mathf.Abs(vec.x) > Mathf.Abs(vec.y))
            return new Vector2(vec.x > 0 ? 1 : -1, 0);
        return new Vector2(0, vec.y > 0 ? 1 : -1);
    }
    protected static Vector2Int MaxContrastInt(Vector2 vec) =>
        Vector2Int.RoundToInt(MaxContrast(vec)); 
    protected static bool CheckPlace(int x, int y, out int error)
    {
        if (!SquareCreator.WalkableMap.GetTile(new Vector3Int(x,y))) //или элемент массива == null
        {
            error = -1;
            return false;
        }
        error = 0;
        if (SquareCreator.map[x, y].currentEntity) return false;
        return true;
    }
    protected bool CheckPlace(int x, int y, bool interact = false)
    {
        bool result = CheckPlace(x, y, out int error);
        if (error != -1 && !result && interact) SquareCreator.map[x, y].currentEntity.Interact(this);
        return result;
    }
    protected void Displace(Vector2Int toWhere)
    {
        int nextX = toWhere.x;
        int nextY = toWhere.y;
        transform.position = new Vector3(nextX * MOVE_DISPLACEMENT, nextY * MOVE_DISPLACEMENT, 0f);
        SquareCreator.map[GridPosition.x, GridPosition.y].currentEntity = null;
        GridPosition.x = nextX;
        GridPosition.y = nextY;
        SquareCreator.map[GridPosition.x, GridPosition.y].currentEntity = this;
    }
    protected void TryDisplace(Vector2Int toWhere, Action onStep = null)
    {
        int nextX = toWhere.x;
        int nextY = toWhere.y;
        if (!CheckPlace(nextX, nextY, out int error))
            if (error != -1)
                SquareCreator.map[nextX, nextY].currentEntity.Interact(this);
            else return;
        else Displace(toWhere);
        onStep?.Invoke();
    }
    /// <summary>
    /// Направляет объект по клеточному полю, в сторону точки
    /// </summary>
    /// <param name="toWhere"></param>
    protected void MoveTowards(Vector2 toWhere)
    {
        if (toWhere != Vector2.negativeInfinity)
        {
            Vector2Int targetPos = GridPosition + MaxContrastInt((toWhere - (Vector2)GridPosition).normalized);
            TryDisplace(targetPos);
        }
    }
    protected void MoveTowards(Entity target) => 
        MoveTowards(target.GridPosition);
}
