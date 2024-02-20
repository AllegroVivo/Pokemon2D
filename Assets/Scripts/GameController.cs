﻿using System;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog }

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private BattleSystem _battleSystem;
    [SerializeField] private Camera _worldCamera;
    
    private GameState _state;

    private void Awake()
    {
        ConditionsDB.Init();
    }

    private void Start()
    {
        _playerController.OnEncountered += StartBattle;
        _battleSystem.OnBattleOver += EndBattle;
        
        DialogManager.I.OnShowDialog += () =>
        {
            _state = GameState.Dialog;
        };
        DialogManager.I.OnCloseDialog += () =>
        {
            if (_state == GameState.Dialog)
                _state = GameState.FreeRoam;
        };
    }

    private void Update()
    {
        if (_state == GameState.FreeRoam)
            _playerController.HandleUpdate();
        else if (_state == GameState.Battle)
            _battleSystem.HandleUpdate();
        else if (_state == GameState.Dialog)
            DialogManager.I.HandleUpdate();
    }

    private void StartBattle()
    {
        _state = GameState.Battle;
        _battleSystem.gameObject.SetActive(true);
        _worldCamera.gameObject.SetActive(false);

        PokemonParty playerParty = _playerController.GetComponent<PokemonParty>();
        Pokemon wildMon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        _battleSystem.StartBattle(playerParty, wildMon);
    }

    private void EndBattle(Boolean battleWon)
    {
        _state = GameState.FreeRoam;
        _battleSystem.gameObject.SetActive(false);
        _worldCamera.gameObject.SetActive(true);
    }
}