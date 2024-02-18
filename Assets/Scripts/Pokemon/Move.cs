using System;
using TMPro.EditorUtilities;
using UnityEngine;

public class Move
{
    public MoveBase Base { get; }

    public String Name => Base.Name;
    public String Description => Base.Description;

    public PokemonType Type => Base.Type;
    public Int32 Power => Base.Power;
    public Int32 Accuracy => Base.Accuracy;
    public Int32 MaxPP => Base.PP;

    public Boolean IsSpecial => Category == MoveCategory.Special;
    public Boolean IsStatus => Category == MoveCategory.Status;
    
    public MoveCategory Category => Base.Category;
    public MoveEffects Effects => Base.Effects;
    public MoveTarget Target => Base.Target;
    
    public Int32 CurrentPP { get; set; }

    public Move(MoveBase mBase)
    {
        Base = mBase;
        CurrentPP = mBase.PP;
    }
}