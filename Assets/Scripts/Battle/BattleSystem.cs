using System;
using System.Collections;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit _playerUnit;
    [SerializeField] private BattleUnit _enemyUnit;

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
        if (_state == BattleState.ActionSelection)
            HandleActionSelection();
        else if (_state == BattleState.MoveSelection)
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
                MoveSelection();
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
            StartCoroutine(PlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            _dialogBox.EnableMoveSelector(false);
            _dialogBox.EnableDialogText(true);
            ActionSelection();
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
            ActionSelection();
        }
    }

    private IEnumerator SetupBattle()
    {
        _playerUnit.Setup(_playerParty.GetHealthyPokemon());
        _enemyUnit.Setup(_wildMon);
        
        _partyScreen.Init();
        _dialogBox.SetMoveNames(_playerUnit.Mon.Moves);
        
        yield return _dialogBox.TypeDialog($"A wild {_enemyUnit.Mon.Name} appeared!");

        ChooseFirstTurn();
    }

    private void ActionSelection()
    {
        _state = BattleState.ActionSelection;
        _dialogBox.SetDialog("Choose an action.");
        _dialogBox.EnableActionSelector(true);
        
    }

    private void MoveSelection()
    {
        _state = BattleState.MoveSelection;
        _dialogBox.EnableActionSelector(false);
        _dialogBox.EnableDialogText(false);
        _dialogBox.EnableMoveSelector(true);
    }

    private IEnumerator PlayerMove()
    {
        _state = BattleState.PerformingMove;
        yield return RunMove(_playerUnit, _enemyUnit, _playerUnit.Mon.Moves[_currentMove]);
        
        if (_state == BattleState.PerformingMove)
            StartCoroutine(EnemyMove());
    }

    private IEnumerator EnemyMove()
    {
        _state = BattleState.PerformingMove;
        yield return RunMove(_enemyUnit, _playerUnit, _enemyUnit.Mon.GetRandomMove());
        
        if (_state == BattleState.PerformingMove)
            ActionSelection();
    }

    private void BattleOver(Boolean battleWon)
    {
        _state = BattleState.BattleOver;
        _playerParty.PartyList.ForEach(p => p.OnBattleOver());
        OnBattleOver?.Invoke(battleWon);
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
        Boolean currentFainted = true;
        if (!_playerUnit.Mon.IsFainted)
        {
            yield return _dialogBox.TypeDialog($"Come back {_playerUnit.Mon.Name}!");

            currentFainted = false;
            _playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        _playerUnit.Setup(newMon);
        _dialogBox.SetMoveNames(newMon.Moves);
        yield return _dialogBox.TypeDialog($"Go {newMon.Name}!");

        if (currentFainted)
            ChooseFirstTurn();
        else
            StartCoroutine(EnemyMove());
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.CurrentPP--;
        yield return _dialogBox.TypeDialog($"{sourceUnit.Mon.Name} used {move.Name}.");
        
        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        targetUnit.PlayHitAnimation();

        if (move.IsStatus)
        {
            yield return RunMoveEffects(move, sourceUnit.Mon, targetUnit.Mon);
        }
        else
        {
            DamageDetails damageDetails = targetUnit.Mon.TakeDamage(move, sourceUnit.Mon);
            yield return targetUnit.HUD.UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }
        
        
        if (targetUnit.Mon.IsFainted)
        {
            yield return _dialogBox.TypeDialog($"{targetUnit.Mon.Name} fainted");
            targetUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);

            CheckForBattleOver(targetUnit);
        }
        
        sourceUnit.Mon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Mon);
        yield return sourceUnit.HUD.UpdateHP();
        
        if (sourceUnit.Mon.IsFainted)
        {
            yield return _dialogBox.TypeDialog($"{sourceUnit.Mon.Name} fainted");
            sourceUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
            
        }
    }

    private void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayer)
        {
            if (_playerParty.GetHealthyPokemon() != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
        {
            BattleOver(true);
        }
    }

    private IEnumerator ShowStatusChanges(Pokemon mon)
    {
        while (mon.StatusChanges.Count > 0)
            yield return _dialogBox.TypeDialog(mon.StatusChanges.Dequeue());
    }

    private void ChooseFirstTurn()
    {
        if (_playerUnit.Mon.Speed >= _enemyUnit.Mon.Speed)
            ActionSelection();
        else
            StartCoroutine(EnemyMove());
    }

    private IEnumerator RunMoveEffects(Move move, Pokemon source, Pokemon target)
    {
        MoveEffects effects = move.Effects;
        if (effects.StatBoosts != null)
        {
            if (move.Target == MoveTarget.Self)
                source.ApplyBoosts(effects.StatBoosts);
            else
                target.ApplyBoosts(effects.StatBoosts);
        }

        if (effects.Status != ConditionID.None)
            target.SetStatus(effects.Status);

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
}