using UnityEngine;

public sealed class EntityStats
{
    public int armour;
    public int evasionChance;
    public int critChance;
    public int critMultiplier;
    public int precision;
    public int maxMana;
    public int maxHp;
    public int manaRegen;
    public int hpRegen;
    public (int Fire, int Cold, int Lightning, int Poison) resists;
    public (int Fire, int Cold, int Lightning, int Poison) damages;
    public int level;
    public int currentExp;
    public int maxExp;
    public int dexterity;
    public int strength;
    public int intelligence;
}
