using UnityEngine;

public sealed class Chest : Entity
{
    public override EntityType TypeOfEntity { get; protected set; }

    public override string Name => "Сундук";
    public new void Awake()
    {
        base.Awake();
        TypeOfEntity = new ConcreteEntity(this);
    }
    protected override Sprite LoadSprite() =>
        Resources.Load<Sprite>("2DSprites/items and trap_animation/chest/chest_3");
}
