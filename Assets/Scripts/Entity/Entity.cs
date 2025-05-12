using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public Vector2Int GridPosition = Vector2Int.zero;
    public abstract void Interact();
}
