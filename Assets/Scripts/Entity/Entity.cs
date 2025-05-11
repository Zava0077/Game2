using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public Vector2Int CellPosition;
    public abstract void Interact();
}
