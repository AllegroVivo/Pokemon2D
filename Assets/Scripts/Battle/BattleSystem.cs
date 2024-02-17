using System;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit _playerUnit;
    [SerializeField] private BattleHUD _playerHUD;
    
    [SerializeField] private BattleUnit _enemyUnit;
    [SerializeField] private BattleHUD _enemyHUD;

    private void Start()
    {
        SetupBattle();
    }

    private void SetupBattle()
    {
        _playerUnit.Setup();
        _enemyUnit.Setup();
        
        _playerHUD.SetData(_playerUnit.Mon);
        _enemyHUD.SetData(_enemyUnit.Mon);
    }
}