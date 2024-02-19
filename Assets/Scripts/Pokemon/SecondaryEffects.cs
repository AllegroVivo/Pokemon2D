using System;
using UnityEngine;

[Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] private Int32 _chance;
    [SerializeField] private MoveTarget _target;

    public Int32 Chance => _chance;
    public MoveTarget Target => _target;
}