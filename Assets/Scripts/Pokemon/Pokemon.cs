using System;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    private PokemonBase _base;
    private Int32 _level;
    
    public String Name => _base.Name;
    public String Description => _base.Description;
    
    public Sprite FrontSprite => _base.FrontSprite;
    public Sprite BackSprite => _base.BackSprite;

    public PokemonType Type1 => _base.Type1;
    public PokemonType Type2 => _base.Type2;

    public Int32 BaseHP => _base.MaxHP;
    public Int32 BaseAttack => _base.Attack;
    public Int32 BaseDefense => _base.Defense;
    public Int32 BaseSpAtk => _base.SpAtk;
    public Int32 BaseSpDef => _base.SpDef;
    public Int32 BaseSpeed => _base.Speed;

    public Int32 MaxHP => Mathf.FloorToInt(BaseHP * _level / 100f) + _level + 10;
    public Int32 CurrentHP { get; set; }
    
    public Int32 Attack => Mathf.FloorToInt(BaseAttack * _level / 100f) + 5;
    public Int32 Defense => Mathf.FloorToInt(BaseDefense * _level / 100f) + 5;
    public Int32 SpAtk => Mathf.FloorToInt(BaseSpAtk * _level / 100f) + 5;
    public Int32 SpDef => Mathf.FloorToInt(BaseSpDef * _level / 100f) + 5;
    public Int32 Speed => Mathf.FloorToInt(BaseSpeed * _level / 100f) + 5;

    public List<LearnableMove> LearnableMoves => _base.LearnableMoves;
    public List<Move> Moves { get; set; }

    public Pokemon(PokemonBase pBase, Int32 pLevel)
    {
        _base = pBase;
        _level = pLevel;
        CurrentHP = MaxHP

        Moves = new List<Move>();
        foreach (LearnableMove move in LearnableMoves)
        {
            if (move.Level <= _level)
                Moves.Add(new Move(move.Base));
            if (Moves.Count >= 4)
                break;
        }
    }
}