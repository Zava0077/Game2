using System.Collections.Generic;
using UnityEngine;

public sealed class WoodenSign : NPC, ILogger
{
    public override EntityType TypeOfEntity { get; protected set; }
    public override string Name => "Табличка";
    public List<LogElement> Targets { get; set; }
    public new void Awake()
    {
        base.Awake();
        TypeOfEntity = new ConcreteEntity(this);
    }
    public new void Start()
    {
        base.Start();
        this.CreateLogTargetsOnTarget(transform, 10);
    }
    public override void Interact(Entity whoInteracts)
    {
        base.Interact(whoInteracts);
        LogManager.Log("Я табличка!", this);
    }
    protected override Sprite LoadSprite() =>
        Resources.Load<Sprite>("2DSprites/WoodenSign");
}
