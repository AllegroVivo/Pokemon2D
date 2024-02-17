using System;
using System.Collections;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit _playerUnit;
    [SerializeField] private BattleHUD _playerHUD;
    
    [SerializeField] private BattleUnit _enemyUnit;
    [SerializeField] private BattleHUD _enemyHUD;

    [SerializeField] private BattleDialogBox _dialogBox;

    public event Action<Boolean> OnBattleOver;

    private Int32 _currentAction;
    private Int32 _currentMove;

    private BattleState _state;
    
    private PokemonParty _playerParty;
    private Pokemon _wildMon;

    public void StartBattle(PokemonParty playerParty, Pokemon wildMon)
    {
        _playerParty = playerParty;
        _wildMon = wildMon;
        
        StartCoroutine(SetupBattle());
    }

    public void HandleUpdate()
    {
        if (_state == BattleState.PlayerAction)
            HandleActionSelection();
        else if (_state == BattleState.PlayerMove)
            HandleMoveSelection();
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_currentAction < 1)
                _currentAction++;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_currentAction > 0)
                _currentAction--;
        }

        _dialogBox.UpdateActionSelection(_currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (_currentAction == 0)
                PlayerMove();
            else if (_currentAction == 1)
            {
                
            }
        }
    }

    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_currentMove < _playerUnit.Mon.Moves.Count - 1)
                _currentMove++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_currentMove > 0)
                _currentMove--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_currentMove < _playerUnit.Mon.Moves.Count - 1)
                _currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_currentMove > 1)
                _currentMove -= 2;
        }

        _dialogBox.UpdateMoveSelection(_currentMove, _playerUnit.Mon.Moves[_currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            _dialogBox.EnableMoveSelector(false);
            _dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }

    private IEnumerator SetupBattle()
    {
        _playerUnit.Setup(_playerParty.GetHealthyPokemon());
        _enemyUnit.Setup(_wildMon);
        
        _playerHUD.SetData(_playerUnit.Mon);
        _enemyHUD.SetData(_enemyUnit.Mon);

        _dialogBox.SetMoveNames(_playerUnit.Mon.Moves);
        
        yield return _dialogBox.TypeDialog($"A wild {_enemyUnit.Mon.Name} appeared!");

        PlayerAction();
    }

    private void PlayerAction()
    {
        _state = BattleState.PlayerAction;
        StartCoroutine(_dialogBox.TypeDialog("Choose an action."));
        _dialogBox.EnableActionSelector(true);
        
    }

    private void PlayerMove()
    {
        _state = BattleState.PlayerMove;
        _dialogBox.EnableActionSelector(false);
        _dialogBox.EnableDialogText(false);
        _dialogBox.EnableMoveSelector(true);
    }

    private IEnumerator PerformPlayerMove()
    {
        _state = BattleState.Busy;
        
        Move move = _playerUnit.Mon.Moves[_currentMove];
        move.CurrentPP--;
        yield return _dialogBox.TypeDialog($"{_playerUnit.Mon.Name} used {move.Name}.");
        
        _playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        _enemyUnit.PlayHitAnimation();
        DamageDetails damageDetails = _enemyUnit.Mon.TakeDamage(move, _playerUnit.Mon);
        yield return _enemyHUD.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return _dialogBox.TypeDialog($"{_enemyUnit.Mon.Name} fainted");
            _enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver?.Invoke(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    private IEnumerator EnemyMove()
    {
        _state = BattleState.EnemyMove;

        Move move = _enemyUnit.Mon.GetRandomMove();
        move.CurrentPP--;
        yield return _dialogBox.TypeDialog($"{_enemyUnit.Mon.Name} used {move.Name}.");
        
        _enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        
        _playerUnit.PlayHitAnimation();
        DamageDetails damageDetails = _playerUnit.Mon.TakeDamage(move, _enemyUnit.Mon);
        yield return _playerHUD.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        
        if (damageDetails.Fainted)
        {
            yield return _dialogBox.TypeDialog($"{_playerUnit.Mon.Name} fainted");
            _playerUnit.PlayFaintAnimation();
            
            yield return new WaitForSeconds(2f);

            Pokemon nextMon = _playerParty.GetHealthyPokemon();
            if (nextMon != null)
            {
                _playerUnit.Setup(nextMon);
                _playerHUD.SetData(nextMon);
                _dialogBox.SetMoveNames(nextMon.Moves);
                
                yield return _dialogBox.TypeDialog($"Go {nextMon.Name}!");
                
                PlayerAction();
            }
            else
            {
                OnBattleOver?.Invoke(false);
            }
        }
        else
        {
            PlayerAction();
        }
    }

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return _dialogBox.TypeDialog("A critical hit!");
        if (damageDetails.TypeEffectiveness > 1f)
            yield return _dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return _dialogBox.TypeDialog("It's not very effective...");
    }
}