using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static SquareCreator;
public class Spider : Enemy
{
    public override string Name { get; } = "Паук";
    public override void OnMove()
    {
        Vector2 target = Vector2.negativeInfinity;
        Vector2 random = directions[rnd.Next(0, directions.Length)];
        float distance = Vector2.Distance(transform.position, Target.transform.position);
        if (distance < 3 && (Player.TimeStep - offset) % 4 != 0)
            target = GridPosition + (Target.transform.position - transform.position).normalized.MaxContrast() * -1; //Run
        else if (distance < 4 && this.Shoot(Target))
            return;
        else if (distance < 10)
            target = GetPath(Target).FirstOrDefault();
        else
            target = random;
        MoveTowards(target);
    }
    protected override Sprite LoadSprite() =>
        Resources.LoadAll<Sprite>("2DSprites/character and tileset")[176];
}

