using System;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class MapArea : MonoBehaviour
{
    [SerializeField] private List<Pokemon> _wildPokemonList;

    public Pokemon GetRandomWildPokemon()
    {
        Pokemon wildMon = _wildPokemonList[URandom.Range(0, _wildPokemonList.Count)];
        wildMon.Init();
        return wildMon;
    }
}