using UnityEngine;

public sealed class EntityStats
{
    public int armour = 0;
    public int evasionChance = 0;
    public int critChance = 0;
    public int critMultiplier = 0;
    public int precision = 0;
    public int maxMana = 0;
    public int maxHp = 0;
    public int manaRegen = 0;
    public int hpRegen = 0;
    public (int Fire, int Cold, int Lightning, int Poison) resists = (0, 0, 0, 0);
    public (int Fire, int Cold, int Lightning, int Poison) damages = (0, 0, 0, 0);
    public int level = 0;
    public int currentExp = 0;
    public int maxExp = 0;
    public int dexterity = 0;
    public int strength = 0;
    public int intelligence = 0;
}
