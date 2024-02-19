using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class MoveEffects
{
    [SerializeField] private List<StatBoost> _boosts;
    [SerializeField] private ConditionID _status;
    [SerializeField] private ConditionID _volatileStatus;
    
    public List<StatBoost> StatBoosts => _boosts;
    public ConditionID Status => _status;
    public ConditionID VolatileStatus => _volatileStatus;
}