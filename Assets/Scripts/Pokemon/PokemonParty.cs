using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> _party;

    private void Start()
    {
        foreach (Pokemon mon in _party)
            mon.Init();
    }

    public Pokemon GetHealthyPokemon() => _party.FirstOrDefault(x => x.CurrentHP > 0);
}