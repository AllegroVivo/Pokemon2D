using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB
{
    private static Dictionary<String, MoveBase> _moves;

    public static void Init()
    {
        _moves = new Dictionary<String, MoveBase>();

        MoveBase[] moveList = Resources.LoadAll<MoveBase>("");
        foreach (MoveBase move in moveList)
        {
            if (!_moves.TryAdd(move.Name, move))
                Debug.LogError($"There are two moves with the same name: '{move.Name}'");
        }
    }

    public static MoveBase GetMoveByName(String name)
    {
        if (_moves.TryGetValue(name, out MoveBase outBase))
            return outBase;
        
        Debug.LogError($"Move with name '{name}' does not exist in the database.");
        return null;
    }
}