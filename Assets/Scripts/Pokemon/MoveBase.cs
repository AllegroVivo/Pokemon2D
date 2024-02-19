using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/New Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private String _name;
    [TextArea] [SerializeField] private String _description;
    
    [SerializeField] private PokemonType _type;
    
    [SerializeField] private Int32 _power;
    [SerializeField] private Int32 _accuracy;
    [SerializeField] private Boolean _alwaysHits;
    [SerializeField] private Int32 _pp;
    
    [SerializeField] private MoveCategory _category;
    [SerializeField] private MoveEffects _effects;
    [SerializeField] private List<SecondaryEffects> _secondaryEffects;
    [SerializeField] private MoveTarget _target;

    public String Name => _name;
    public String Description => _description;

    public PokemonType Type => _type;
    public Int32 Power => _power;
    public Int32 Accuracy => _accuracy;
    public Boolean AlwaysHits => _alwaysHits;
    public Int32 PP => _pp;

    public MoveCategory Category => _category;

    public MoveEffects Effects => _effects;
    public List<SecondaryEffects> SecondaryEffects => _secondaryEffects;
    public MoveTarget Target => _target;
}
