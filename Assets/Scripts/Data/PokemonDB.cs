using System;
using System.Collections.Generic;
using UnityEngine;

public class PokemonDB
{
    private static Dictionary<String, PokemonBase> _pokemonList;

    public static void Init()
    {
        _pokemonList = new Dictionary<String, PokemonBase>();
        
        PokemonBase[] monArray = Resources.LoadAll<PokemonBase>("");
        foreach (PokemonBase mon in monArray)
        {
            if (_pokemonList.ContainsKey(mon.Name))
            {
                Debug.LogError($"There are two Pokemon objects with the name '{mon.Name}'");
            }
            
            _pokemonList[mon.Name] = mon;
        }
    }

    public static PokemonBase GetPokemonByName(String name)
    {
        if (_pokemonList.TryGetValue(name, out PokemonBase outBase))
            return outBase;
            
        Debug.LogError($"A Pokemon with the name {name} could not be found in the database.");
        return null;
    }
}