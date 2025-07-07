using UnityEngine;

public struct EntityStats
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

    public EntityStats(
        int armour = 0,
        int evasionChance = 0,
        int critChance = 0,
        int critMultiplier = 0,
        int precision = 0,
        int maxMana = 0,
        int maxHp = 0,
        int manaRegen = 0,
        int hpRegen = 0,
        (int Fire, int Cold, int Lightning, int Poison)? resists = null,
        (int Fire, int Cold, int Lightning, int Poison)? damages = null,
        int level = 0,
        int currentExp = 0,
        int maxExp = 0,
        int dexterity = 0,
        int strength = 0,
        int intelligence = 0)
    {
        this.armour = armour;
        this.evasionChance = evasionChance;
        this.critChance = critChance;
        this.critMultiplier = critMultiplier;
        this.precision = precision;
        this.maxMana = maxMana;
        this.maxHp = maxHp;
        this.manaRegen = manaRegen;
        this.hpRegen = hpRegen;
        this.resists = resists ?? (0, 0, 0, 0);
        this.damages = damages ?? (0, 0, 0, 0);
        this.level = level;
        this.currentExp = currentExp;
        this.maxExp = maxExp;
        this.dexterity = dexterity;
        this.strength = strength;
        this.intelligence = intelligence;
    }
}
