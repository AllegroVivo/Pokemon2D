using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatBoost> _boosts;
    
    public List<StatBoost> StatBoosts => _boosts;
}