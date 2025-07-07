using UnityEngine;

public sealed class WoodenSign : NPC
{
    public override EntityType TypeOfEntity { get; protected set; }
    public override string Name => "Табличка";
    public new void Awake()
    {
        base.Awake();
        TypeOfEntity = new ConcreteEntity(this);
    }
    protected override Sprite LoadSprite() =>
        Resources.Load<Sprite>("2DSprites/WoodenSign");
}
