using AStar;
using System.Collections.Generic;
using UnityEngine;
using static MovementService;
using static SquareCreator;
using static AStar.BackTrackingAStar;
using System.Linq;
public class Marker : Entity
{
    public override EntityType TypeOfEntity { get; protected set; }
    public override RenderLevels? RenderLevel { get; internal set; } = RenderLevels.Effects;
    public override string Name => null;
    private new void Awake()
    {
        base.Awake();
        Player.OnCheckTick += OnTick;
        TypeOfEntity = new AbstractEntity(this);
    }
    public override void Interact(Entity whoInteracts)
    {
        if (CheckPlace(GridPosition.x, GridPosition.y, out int error) is not null
            && CountDistanceDiagonally(GridPosition.x, GridPosition.y, Player.character.GridPosition.x, Player.character.GridPosition.y) == 1)
            if (whoInteracts.CanSee(map[GridPosition.x, GridPosition.y].currentEntity))
                map[GridPosition.x, GridPosition.y].currentEntity.Interact(whoInteracts);
            else
                whoInteracts.Shoot(map[GridPosition.x, GridPosition.y].currentEntity);
        else
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
