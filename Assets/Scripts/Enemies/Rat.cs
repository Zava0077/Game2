using System.Xml;
using UnityEngine;

public class Rat : Enemy
{
    public override void OnMove()
    {
        if ((Player.TimeStep - offset) == 0) return;
        Vector2 target = Vector2.negativeInfinity;
        Vector2 random = directions[rnd.Next(0, directions.Length)];
        float distance = Vector2.Distance(transform.position, Player.character.transform.position);
        if(distance < 2)
            target = GridPosition + MaxContrast((Player.character.transform.position - transform.position).normalized) * -1;
        else if (distance < 5)
            target = Player.character.transform.position;
        else if (new System.Random().Next(0, 2) < 1)
            target = GridPosition + random;
        MoveTowards(target);//
    }
    private new void Start()
    {
        base.Start();
    }
}
