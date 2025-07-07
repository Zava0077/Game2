using System;
using System.Collections;
using UnityEngine;
using static Player;
public abstract class PlayerState
{
    public PlayerState(Player actualPlayer)
    {
        ActualPlayer = actualPlayer;
    }

    public Player ActualPlayer { get; protected set; }
    public abstract void Action();
}
public sealed class FreeRoaming : PlayerState
{
    public FreeRoaming(Player actualPlayer) : base(actualPlayer)
    {
        
    }

    public override void Action()
    {
        ActualPlayer.InvokeOnCheckTick();
        if (Input.GetMouseButton(0))
        {
            ActualPlayer.playerMarker.Interact(ActualPlayer);
            ActualPlayer.InvokeOnTimeStep();
        }
    }
}
public sealed class Stunned : PlayerState
{
    public Stunned(Player actualPlayer) : base(actualPlayer)
    {
    }

    public override void Action()
    {
        ActualPlayer.InvokeOnCheckTick();
        ActualPlayer.InvokeOnTimeStep();
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