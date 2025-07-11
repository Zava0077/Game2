using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;
using System.Linq;
using System.Collections.Generic;
using TMPro;
public sealed class ChestMimic : Enemy
{
    public override EntityType TypeOfEntity { get; protected set; }
    public MimicState Sleeping { get; private set; }
    public MimicState Awaked { get; private set; }
    public MimicState mimicState;
    public override string Name => mimicState.VisibleName;
    public new void Awake()
    {
        base.Awake();
        Sleeping = new SleepingMimicState(this);
        Awaked = new AgressiveMimicState(this);
        TypeOfEntity = new ConcreteEntity(this);
        mimicState = Sleeping;
    }
    public override void OnMove()
    {
        mimicState.Action();
    }
    protected override Sprite LoadSprite() =>
        Resources.Load<Sprite>("2DSprites/items and trap_animation/chest/chest_3");
}
public abstract class MimicState : EntityState<ChestMimic>
{
    protected MimicState(ChestMimic entity) : base(entity)
    {
    }

    public abstract string VisibleName { get; }
}
public sealed class AgressiveMimicState : MimicState
{
    private int withoutAttackCounter = 0;
    public AgressiveMimicState(ChestMimic chestMimic) : base(chestMimic)
    {
    }
    public override string VisibleName => "Мимик";
    public override void Action()
    {
        if ((Player.TimeStep - ActualEntity.offset) % 2 == 0) return;
        Vector2 target = Vector2.negativeInfinity;
        Vector2 random = ActualEntity.directions[ActualEntity.rnd.Next(0, ActualEntity.directions.Length)];
        float distance = ActualEntity.GridPosition.DiagonalDistance(ActualEntity.Target.GridPosition);
        if (distance < 5)
            target = ActualEntity.GetPath(ActualEntity.Target).FirstOrDefault();
        else if (new System.Random().Next(0, 2) < 1)
        {
            target = ActualEntity.GridPosition + random;
            withoutAttackCounter++;
        }
        if (target != default)
            ActualEntity.MoveTowards(target);
        if (withoutAttackCounter > 5)
        {
            ActualEntity.mimicState = ActualEntity.Sleeping;
            withoutAttackCounter = 0;
        }
    }
}
public sealed class SleepingMimicState : MimicState
{
    public SleepingMimicState(ChestMimic chestMimic) : base(chestMimic)
    {

    }

    public override string VisibleName => "Сундук";
    public override void Action()
    {
        float distance = Vector2.Distance(ActualEntity.transform.position, ActualEntity.Target.transform.position);
        if (distance < 3) ActualEntity.mimicState = ActualEntity.Awaked;
    }
}