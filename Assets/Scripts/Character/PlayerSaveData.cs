using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public Single[] position;
    public List<PokemonSaveData> party;
}