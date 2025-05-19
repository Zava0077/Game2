using System;
using UnityEngine;

public class Skeleton : Enemy
{
    public override void OnMove()
    {
        Vector2 target = Vector2.negativeInfinity;
        Vector2 random = directions[rnd.Next(0, directions.Length)];
        float distance = Vector2.Distance(transform.position, Player.character.transform.position);
        if (distance < 5 && (Player.TimeStep - offset) % 3 != 0)
            target = GridPosition + (Player.character.transform.position - transform.position).normalized.MaxContrast() * -1;
        else if (distance < 7)
        {
            if ((Player.TimeStep - offset) % 2 != 0)
                Player.character.Interact(this);
            return;
        }
        else if (distance < 8)
            target = Player.character.transform.position;
        else
            target = random;
        MoveTowards(target);
    }
    protected override Sprite LoadSprite() =>
        Resources.LoadAll<Sprite>("2DSprites/character and tileset")[179];
}
