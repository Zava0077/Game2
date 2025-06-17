using AStar;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
using static SquareCreator;
using static MovementService;
using System.Diagnostics.Tracing;
using System.Collections;
public class Marker : Entity
{
    public override EntityType TypeOfEntity { get; protected set; }
    private new void Awake()
    {
        base.Awake();
        Player.OnCheckTick += OnTick;
        TypeOfEntity = new AbstractEntity(this);
    }
    public override void Interact(Entity whoInteracts)
    {
        //if (CheckPlace(GridPosition.x, GridPosition.y, out int error) is Entity and not null
        //    && BackTrackingAStar.CountDistance(GridPosition.x, GridPosition.y, Player.character.GridPosition.x, Player.character.GridPosition.y) == 1)
        //    SquareCreator.map[GridPosition.x, GridPosition.y].currentEntity.Interact(whoInteracts);
        //else
        whoInteracts.MoveTowards(this);
    }


    private void OnTick()
    {
        Vector3 mouseWorldPos = Player.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = WalkableMap.WorldToCell(mouseWorldPos);
        MovementService.VisualDisplace((Vector2Int) cellPos);
        if (!CheckPlace(cellPos.x, cellPos.y, out int error))
            return;
    }
    protected override Sprite LoadSprite() =>
        Resources.Load<Sprite>("2DSprites/Marker");
}
