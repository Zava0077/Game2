using System;
using System.Xml;
using UnityEngine;

public class Rat : Enemy
{
    public override void OnMove()
    {
        if ((Player.TimeStep - offset) % 2 == 0) return;
        Vector2 target = Vector2.negativeInfinity;
        Vector2 random = directions[rnd.Next(0, directions.Length)];
        float distance = Vector2.Distance(transform.position, Player.character.transform.position);
        if (distance < 2 && Player.character.Direction == -Direction)
            target = GridPosition + (Player.character.transform.position - transform.position).normalized.MaxContrast() * -1;
        else if (distance < 5)
            target = Player.character.transform.position;
        else if (new System.Random().Next(0, 2) < 1)
            target = GridPosition + random;
        MoveTowards(target);
    }
    protected override Sprite LoadSprite() =>
        Resources.LoadAll<Sprite>("2DSprites/character and tileset")[175];
}
