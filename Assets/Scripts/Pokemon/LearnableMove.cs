using System;
using UnityEngine;

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase _moveBase;
    [SerializeField] private Int32 _level;

    public MoveBase Base => _moveBase;
    public Int32 Level => _level;
}
