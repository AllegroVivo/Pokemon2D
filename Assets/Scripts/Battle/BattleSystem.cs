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
    [SerializeField] private PartyScreen _partyScreen;

    public event Action<Boolean> OnBattleOver;

    private Int32 _currentAction;
    private Int32 _currentMove;
    private Int32 _currentMember;

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
        else if (_state == BattleState.PartyScreen)
            HandlePartySelection();
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            _currentAction++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            _currentAction--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            _currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            _currentAction -= 2;

        _currentAction = Mathf.Clamp(_currentAction, 0, 3);

        _dialogBox.UpdateActionSelection(_currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (_currentAction == 0)
                PlayerMove();
            else if (_currentAction == 1)
            {
                // Bag
            }
            else if (_currentAction == 2)
                OpenPartyScreen();
            else if (_currentAction == 3)
            {
                // Run
            }
        }
    }

    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            _currentMove++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            _currentMove--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            _currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            _currentMove -= 2;

        _currentMove = Mathf.Clamp(_currentMove, 0, _playerUnit.Mon.Moves.Count - 1);

        _dialogBox.UpdateMoveSelection(_currentMove, _playerUnit.Mon.Moves[_currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            _dialogBox.EnableMoveSelector(false);
            _dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            _dialogBox.EnableMoveSelector(false);
            _dialogBox.EnableDialogText(true);
            PlayerAction();
        }
    }

    private void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            _currentMember++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            _currentMember--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            _currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            _currentMember -= 2;

        _currentMember = Mathf.Clamp(_currentMember, 0, _playerParty.PartyList.Count - 1);
        _partyScreen.UpdateMemberSelection(_currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Pokemon selectedMember = _playerParty.PartyList[_currentMember];
            if (selectedMember.IsFainted)
            {
                _partyScreen.SetMessageText($"{selectedMember.Name} is fainted and can't fight!");
                return;
            }

            if (selectedMember == _playerUnit.Mon)
            {
                _partyScreen.SetMessageText($"{selectedMember.Name} is already in battle!");
                return;
            }

            _partyScreen.gameObject.SetActive(false);
            _state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            _partyScreen.gameObject.SetActive(false);
            PlayerAction();
        }
    }

    private IEnumerator SetupBattle()
    {
        _playerUnit.Setup(_playerParty.GetHealthyPokemon());
        _enemyUnit.Setup(_wildMon);
        
        _playerHUD.SetData(_playerUnit.Mon);
        _enemyHUD.SetData(_enemyUnit.Mon);
        
        _partyScreen.Init();
        _dialogBox.SetMoveNames(_playerUnit.Mon.Moves);
        
        yield return _dialogBox.TypeDialog($"A wild {_enemyUnit.Mon.Name} appeared!");

        PlayerAction();
    }

    private void PlayerAction()
    {
        _state = BattleState.PlayerAction;
        _dialogBox.SetDialog("Choose an action.");
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

            if (_playerParty.GetHealthyPokemon() != null)
                OpenPartyScreen();
            else
                OnBattleOver?.Invoke(false);
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

    private void OpenPartyScreen()
    {
        _state = BattleState.PartyScreen;
        _partyScreen.SetPartyData(_playerParty.PartyList);
        _partyScreen.gameObject.SetActive(true);
    }

    private IEnumerator SwitchPokemon(Pokemon newMon)
    {
        if (!_playerUnit.Mon.IsFainted)
        {
            yield return _dialogBox.TypeDialog($"Come back {_playerUnit.Mon.Name}!");

            _playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        _playerUnit.Setup(newMon);
        _playerHUD.SetData(newMon);

        _dialogBox.SetMoveNames(newMon.Moves);
        yield return _dialogBox.TypeDialog($"Go {newMon.Name}!");

        StartCoroutine(EnemyMove());
    }
}