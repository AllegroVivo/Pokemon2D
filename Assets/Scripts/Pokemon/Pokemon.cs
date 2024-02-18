using System;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

[Serializable]
public class Pokemon
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private Int32 _level;

    public PokemonBase Base => _base;
    public Int32 Level => _level;

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
    public Boolean IsFainted => CurrentHP <= 0;
    
    public Int32 Attack => Mathf.FloorToInt(BaseAttack * Level / 100f) + 5;
    public Int32 Defense => Mathf.FloorToInt(BaseDefense * Level / 100f) + 5;
    public Int32 SpAtk => Mathf.FloorToInt(BaseSpAtk * Level / 100f) + 5;
    public Int32 SpDef => Mathf.FloorToInt(BaseSpDef * Level / 100f) + 5;
    public Int32 Speed => Mathf.FloorToInt(BaseSpeed * Level / 100f) + 5;

    public List<LearnableMove> LearnableMoves => Base.LearnableMoves;
    public List<Move> Moves { get; set; }

    public void Init()
    {
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

    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        Single critical = URandom.value * 100f <= 6.25 ? 2f : 1f;
        Single typeEffectiveness = 
            TypeChart.GetEffectiveness(move.Type, Type1) * TypeChart.GetEffectiveness(move.Type, Type2);

        DamageDetails damageDetails = new()
        {
            TypeEffectiveness = typeEffectiveness,
            Critical = critical,
            Fainted = false
        };

        Single attack = move.IsSpecial ? attacker.SpAtk : attacker.Attack;
        Single defense = move.IsSpecial ? SpDef : Defense;
        
        Single modifiers = URandom.Range(0.85f, 1f) * typeEffectiveness * critical;
        Single a = (2 * attacker.Level + 10) / 250f;
        Single d = a * move.Power * (attack / defense) + 2;
        Int32 damage = Mathf.FloorToInt(d * modifiers);

        CurrentHP -= damage;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            damageDetails.Fainted = true;
        }

        return damageDetails;
    }

    public Move GetRandomMove() => Moves[URandom.Range(0, Moves.Count)];
}