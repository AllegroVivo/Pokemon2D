using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private Int32 _level;
    [SerializeField] private Boolean _isPlayerUnit;

    public Pokemon Mon { get; set; }
    
    public void Setup()
    {
        Mon = new Pokemon(_base, _level);
        GetComponent<Image>().sprite = _isPlayerUnit ? Mon.BackSprite : Mon.FrontSprite;
    }
}