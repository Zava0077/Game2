using UnityEngine;
public interface ILightning
{
    public int LightRadius { get; }
    public Color LightColor { get; }
}
public class Torch : Entity, ILightning, IFogRevealer
{
    public override EntityType TypeOfEntity { get; protected set; }
    public override string Name => "Факел";
    public int LightRadius => 5;
    public int RevealRadius => LightRadius + 2;
    public Color LightColor => Color.yellow;
    private new void Awake()
    {
        base.Awake();
        TypeOfEntity = new ConcreteEntity(this);
    }
    public override void Interact(Entity whoInteracts)
    {
        base.Interact(whoInteracts);
        MoveTowards(GridPosition + (GridPosition - whoInteracts.GridPosition));
    }
    protected override Sprite LoadSprite() =>
        Resources.Load<Sprite>("2DSprites/items and trap_animation/torch/Candlestick_1_3");
}
