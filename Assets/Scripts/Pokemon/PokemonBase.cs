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
}
