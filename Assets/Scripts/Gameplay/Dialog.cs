using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialog
{
    [SerializeField] private List<String> _lines;

    public List<String> Lines => _lines;
}