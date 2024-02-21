using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/New Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private String _name;
    [TextArea] [SerializeField] private String _description;

    [SerializeField] private Sprite _frontSprite;
    [SerializeField] private Sprite _backSprite;

    [SerializeField] private PokemonType _type1;
    [SerializeField] private PokemonType _type2;

    [SerializeField] private Int32 _maxHP;
    [SerializeField] private Int32 _attack;
    [SerializeField] private Int32 _defense;
    [SerializeField] private Int32 _spAtk;
    [SerializeField] private Int32 _spDef;
    [SerializeField] private Int32 _speed;

    [SerializeField] private List<LearnableMove> _learnableMoves;

    [SerializeField] private Int32 _catchRate = 255;
    [SerializeField] private Int32 _expYield;
    
    [SerializeField] private GrowthRate _growthRate;

    public String Name => _name;
    public String Description => _description;
    
    public Sprite FrontSprite => _frontSprite;
    public Sprite BackSprite => _backSprite;

    public PokemonType Type1 => _type1;
    public PokemonType Type2 => _type2;

    public Int32 MaxHP => _maxHP;
    public Int32 Attack => _attack;
    public Int32 Defense => _defense;
    public Int32 SpAtk => _spAtk;
    public Int32 SpDef => _spDef;
    public Int32 Speed => _speed;

    public List<LearnableMove> LearnableMoves => _learnableMoves;

    public Int32 CatchRate => _catchRate;
    public Int32 EXPYield => _expYield;

    public GrowthRate GrowthRate => _growthRate;

    public Int32 GetEXPForLevel(Int32 level)
    {
        return _growthRate switch
        {
            GrowthRate.Fast => 4 * level * level * level / 5,
            GrowthRate.MediumFast => level * level * level,
            GrowthRate.MediumSlow => 6 / 5 * level * level * level - 15 * level * level + 100 * level - 140,
            GrowthRate.Slow => 5 * level * level * level / 4,
            _ => throw new NotImplementedException("EXP Type not implemented.")
        };
    }
}
