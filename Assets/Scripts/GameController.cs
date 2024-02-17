using System;
using UnityEngine;

public enum GameState { FreeRoam, Battle }

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private BattleSystem _battleSystem;
    [SerializeField] private Camera _worldCamera;
    
    private GameState _state;

    private void Start()
    {
        _playerController.OnEncountered += StartBattle;
        _battleSystem.OnBattleOver += EndBattle;
    }

    private void Update()
    {
        if (_state == GameState.FreeRoam)
            _playerController.HandleUpdate();
        else if (_state == GameState.Battle)
            _battleSystem.HandleUpdate();
    }

    private void StartBattle()
    {
        _state = GameState.Battle;
        _battleSystem.gameObject.SetActive(true);
        _worldCamera.gameObject.SetActive(false);
        
        _battleSystem.StartBattle();
    }

    private void EndBattle(Boolean battleWon)
    {
        _state = GameState.FreeRoam;
        _battleSystem.gameObject.SetActive(false);
        _worldCamera.gameObject.SetActive(true);
    }
}