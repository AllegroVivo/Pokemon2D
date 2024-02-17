using System;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class Pokemon
{
    public PokemonBase Base { get; private set; }
    public Int32 Level { get; set; }

    public String Name => Base.Name;
    public String Description => Base.Description;
    
    public Sprite FrontSprite => Base.FrontSprite;
    public Sprite BackSprite => Base.BackSprite;

    public PokemonType Type1 => Base.Type1;
    public PokemonType Type2 => Base.Type2;

    public Int32 BaseHP => Base.MaxHP;
    public Int32 BaseAttack => Base.Attack;
    public Int32 BaseDefense => Base.Defense;
    public Int32 BaseSpAtk => Base.SpAtk;
    public Int32 BaseSpDef => Base.SpDef;
    public Int32 BaseSpeed => Base.Speed;

    public Int32 MaxHP => Mathf.FloorToInt(BaseHP * Level / 100f) + Level + 10;
    public Int32 CurrentHP { get; set; }
    
    public Int32 Attack => Mathf.FloorToInt(BaseAttack * Level / 100f) + 5;
    public Int32 Defense => Mathf.FloorToInt(BaseDefense * Level / 100f) + 5;
    public Int32 SpAtk => Mathf.FloorToInt(BaseSpAtk * Level / 100f) + 5;
    public Int32 SpDef => Mathf.FloorToInt(BaseSpDef * Level / 100f) + 5;
    public Int32 Speed => Mathf.FloorToInt(BaseSpeed * Level / 100f) + 5;

    public List<LearnableMove> LearnableMoves => Base.LearnableMoves;
    public List<Move> Moves { get; set; }

    public Pokemon(PokemonBase pBase, Int32 pLevel)
    {
        Base = pBase;
        Level = pLevel;
        CurrentHP = MaxHP;

        Moves = new List<Move>();
        foreach (LearnableMove move in LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));
            if (Moves.Count >= 4)
                break;
        }
    }

    public Boolean TakeDamage(Move move, Pokemon attacker)
    {
        Single modifiers = URandom.Range(0.85f, 1f);
        Single a = (2 * attacker.Level + 10) / 250f;
        Single d = a * move.Power * ((Single)attacker.Attack / Defense) + 2;
        Int32 damage = Mathf.FloorToInt(d * modifiers);

        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            return true;
        }

        return false;
    }

    public Move GetRandomMove() => Moves[URandom.Range(0, Moves.Count)];
}