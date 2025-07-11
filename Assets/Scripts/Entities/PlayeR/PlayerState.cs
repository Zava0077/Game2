using System;
using System.Collections;
using UnityEngine;
using static Player;
public abstract class EntityState<T> where T : Entity
{
    public T ActualEntity { get; protected set; }
    public EntityState(T entity) => ActualEntity = entity;
    public abstract void Action();
}
public abstract class PlayerState : EntityState<Player>
{
    public PlayerState(Player actualPlayer) : base(actualPlayer) { }
}
public sealed class FreeRoaming : PlayerState
{
    public FreeRoaming(Player actualPlayer) : base(actualPlayer) { }
    public override void Action()
    {
        ActualEntity.InvokeOnCheckTick();
        if(Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            ActualEntity.playerMarker.Goto(ActualEntity.GridPosition + new Vector2Int((int)Input.GetAxisRaw("Horizontal"), (int)Input.GetAxisRaw("Vertical")));
            ActualEntity.playerMarker.Interact(ActualEntity);
            ActualEntity.InvokeOnTimeStep();
        }
        if (Input.GetMouseButton(0))
        {
            ActualEntity.playerMarker.Interact(ActualEntity);
            ActualEntity.InvokeOnTimeStep();
        }
    }
}
public sealed class Stunned : PlayerState
{
    public Stunned(Player actualPlayer) : base(actualPlayer) { }
    public override void Action()
    {
        ActualEntity.InvokeOnCheckTick();
        ActualEntity.InvokeOnTimeStep();
    }
}

public sealed class Talking : PlayerState
{
    public DialogNode dialog; 
    public Talking(Player actualPlayer, DialogNode dialog) : base(actualPlayer)
    {
        this.dialog = dialog;
    }

    public override void Action()
    {
        LogManager.Log("Ѕла-бла-бла!");
    }
}