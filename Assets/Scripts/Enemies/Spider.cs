using System;
using UnityEngine;

public class Spider : Enemy
{

    public override void OnMove()
    {
        Vector2 target = Vector2.negativeInfinity;
        Vector2 random = directions[rnd.Next(0, directions.Length)];
        float distance = Vector2.Distance(transform.position, Player.character.transform.position);
        if (distance < 4 && (Player.TimeStep - offset) % 4 != 0)
            target = GridPosition + (Player.character.transform.position - transform.position).normalized.MaxContrast() * -1;
        else if (distance < 5)
        {
            if ((Player.TimeStep - offset) % 2 != 0)
                Player.character.Interact(this);
            return;
        }
        else if (distance < 6)
            target = Player.character.transform.position;
        //else if(rnd.Next(0,100) < 25)
        //    Player.character.Interact(this);
        else
            target = random;
        MoveTowards(target);
    }
    protected override Sprite LoadSprite() =>
        Resources.LoadAll<Sprite>("2DSprites/character and tileset")[176];
}

