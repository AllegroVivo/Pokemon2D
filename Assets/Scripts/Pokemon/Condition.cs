using System;
using UnityEngine;

public class Condition
{
    public String Name { get; set; }
    public String ShortName { get; set; }
    public String Description { get; set; }
    public String StartMessage { get; set; }

    public Action<Pokemon> OnStart { get; set; }
    public Func<Pokemon, Boolean> OnBeforeMove { get; set; }
    public Action<Pokemon> OnAfterTurn { get; set; }
}