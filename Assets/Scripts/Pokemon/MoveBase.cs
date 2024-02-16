using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/New Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private String _name;
    [TextArea] [SerializeField] private String _description;
    
    [SerializeField] private PokemonType _type;
    [SerializeField] private Int32 _power;
    [SerializeField] private Int32 _accuracy;
    [SerializeField] private Int32 _pp;

    public String Name => _name;
    public String Description => _description;

    public PokemonType Type => _type;
    public Int32 Power => _power;
    public Int32 Accuracy => _accuracy;
    public Int32 PP => _pp;
}
