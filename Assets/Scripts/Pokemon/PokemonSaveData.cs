using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PokemonSaveData
{
    public String name;
    public Int32 currentHP;
    public Int32 level;
    public Int32 currentEXP;
    public ConditionID? status;
    public List<MoveSaveData> moves;
}