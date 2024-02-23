using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> _party;

    public List<Pokemon> PartyList
    {
        get => _party;
        set => _party = value;
    }
    
    private void Start()
    {
        foreach (Pokemon mon in _party)
            mon.Init();
    }

    public Pokemon GetHealthyPokemon() => _party.FirstOrDefault(x => x.CurrentHP > 0);

    public void AddPokemon(Pokemon newMon)
    {
        if (_party.Count < 6)
            _party.Add(newMon);
        else
        {
            // Transfer to PC~
        }
    }
}