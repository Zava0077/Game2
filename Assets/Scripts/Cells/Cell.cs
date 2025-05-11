using System.Xml;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2Int GridPosition;
    public Entity CurrentEntity;


    public void Place(Entity entity)
    {
        CurrentEntity = entity;
        entity.transform.position = this.transform.position;
    }
}
