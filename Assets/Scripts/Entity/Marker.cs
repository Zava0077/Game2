using UnityEngine;

public class Marker : Entity
{
    private new void Awake()
    {
        base.Awake();
        Player.OnCheckTick += OnTick;
    }
    private void OnTick()
    {
        Vector3 mouseWorldPos = Player.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int inputDir = Player.character.GridPosition + Vector2Int.RoundToInt((mouseWorldPos - Player.character.transform.position).normalized.MaxContrast());
        MovementService.VisualDisplace(inputDir);
        if (!MovementService.CheckPlace(inputDir.x, inputDir.y, out int error))
            ;
    }
    protected override Sprite LoadSprite() =>
        Resources.Load<Sprite>("2DSprites/Marker");
}
