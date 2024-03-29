﻿using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using URandom = UnityEngine.Random;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit _playerUnit;
    [SerializeField] private BattleUnit _enemyUnit;

    [SerializeField] private BattleDialogBox _dialogBox;
    [SerializeField] private PartyScreen _partyScreen;

    [SerializeField] private Image _playerImage;
    [SerializeField] private Image _trainerImage;

    [SerializeField] private GameObject _pokeballPrefab;
    [SerializeField] private MoveSelectionUI _moveSelectionUI;

    public event Action<Boolean> OnBattleOver;

    private Int32 _currentAction;
    private Int32 _currentMove;
    private Boolean _aboutToUseChoice = true;

    private Int32 _escapeAttempts;
    private MoveBase _moveToLearn;
    
    private BattleState _state;
    
    private PokemonParty _playerParty;
    private PokemonParty _trainerParty;
    private Pokemon _wildMon;
    private Boolean _isTrainerBattle = false;

    private PlayerController _playerController;
    private TrainerController _trainerController;

    public void StartBattle(PokemonParty playerParty, Pokemon wildMon)
    {
        _playerParty = playerParty;
        _wildMon = wildMon;

        _playerController = _playerParty.GetComponent<PlayerController>();
        _isTrainerBattle = false;
        
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
        else if (_state == BattleState.AboutToUse)
            HandleAboutToUse();
        else if (_state == BattleState.MoveToForget)
        {
            Action<Int32> onMoveSelected = moveIndex =>
            {
                _moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == PokemonBase.MaxNumberOfMoves)
                {
                    StartCoroutine(_dialogBox.TypeDialog($"{_playerUnit.Mon.Name} did not learn {_moveToLearn.Name}"));
                }
                else
                {
                    Move selectedMove = _playerUnit.Mon.Moves[moveIndex];
                    StartCoroutine(_dialogBox.TypeDialog($"{_playerUnit.Mon.Name} forgot {selectedMove.Name}..."));
                    StartCoroutine(_dialogBox.TypeDialog($"and learned {_moveToLearn.Name}"));
                    
                    _playerUnit.Mon.Moves[moveIndex] = new Move(_moveToLearn);
                }

                _moveToLearn = null;
                _state = BattleState.RunningTurn;
            };
            _moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
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
                StartCoroutine(RunTurns(BattleAction.UseItem));
            else if (_currentAction == 2)
                OpenPartyScreen();
            else if (_currentAction == 3)
                StartCoroutine(RunTurns(BattleAction.Run));
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
            if (_playerUnit.Mon.Moves[_currentMove].CurrentPP <= 0)
                return;
            
            _dialogBox.EnableMoveSelector(false);
            _dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
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
        Action onSelected = () =>
        {
            Pokemon selectedMember = _partyScreen.SelectedMember;
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

            if (_partyScreen.CalledFrom == BattleState.ActionSelection)
            {
                StartCoroutine(RunTurns(BattleAction.SwitchMon));
            }
            else
            {
                _state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember, _partyScreen.CalledFrom == BattleState.AboutToUse));
            }

            _partyScreen.CalledFrom = null;
        };
        Action onBack = () =>
        {
            if (_playerUnit.Mon.IsFainted)
            {           
                _partyScreen.SetMessageText("You must choose a Pokemon to continue");
                return;
            }
            
            _partyScreen.gameObject.SetActive(false);

            if (_partyScreen.CalledFrom == BattleState.AboutToUse)
                StartCoroutine(SendNextTrainerPokemon());
            else
                ActionSelection();
            
            _partyScreen.CalledFrom = null;
        };
        _partyScreen.HandleUpdate(onSelected, onBack);
    }

    private void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            _aboutToUseChoice = !_aboutToUseChoice;

        _dialogBox.UpdateChoiceBoxSelection(_aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            _dialogBox.EnableChoiceBox(false);
            if (_aboutToUseChoice)
                OpenPartyScreen();
            else
                StartCoroutine(SendNextTrainerPokemon());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            _dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerPokemon());
        }
    }

    private IEnumerator SetupBattle()
    {
        _playerUnit.Clear();
        _enemyUnit.Clear();
        
        if (!_isTrainerBattle)
        {
            _playerUnit.Setup(_playerParty.GetHealthyPokemon());
            _enemyUnit.Setup(_wildMon);
            _dialogBox.SetMoveNames(_playerUnit.Mon.Moves);
        
            yield return _dialogBox.TypeDialog($"A wild {_enemyUnit.Mon.Name} appeared!");
            
        }
        else
        {
            _playerUnit.gameObject.SetActive(false);
            _enemyUnit.gameObject.SetActive(false);

            _playerImage.gameObject.SetActive(true);
            _trainerImage.gameObject.SetActive(true);

            _playerImage.sprite = _playerController.Sprite;
            _trainerImage.sprite = _trainerController.Sprite;

            yield return _dialogBox.TypeDialog($"{_trainerController.Name} wants to battle!");

            _trainerImage.gameObject.SetActive(false);
            _enemyUnit.gameObject.SetActive(true);
            
            _enemyUnit.Setup(_trainerParty.GetHealthyPokemon());
            yield return _dialogBox.TypeDialog($"{_trainerController.Name} sent out {_enemyUnit.Mon.Name}");

            _playerImage.gameObject.SetActive(false);
            _playerUnit.gameObject.SetActive(true);
            
            _playerUnit.Setup(_playerParty.GetHealthyPokemon());
            yield return _dialogBox.TypeDialog($"Go, {_playerUnit.Mon.Name}!");

            _dialogBox.SetMoveNames(_playerUnit.Mon.Moves);
        }
        
        _escapeAttempts = 0;

        _partyScreen.Init();
        ActionSelection();
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
        _partyScreen.CalledFrom = _state;
        _state = BattleState.PartyScreen;
        _partyScreen.SetPartyData(_playerParty.PartyList);
        _partyScreen.gameObject.SetActive(true);
    }

    private IEnumerator SwitchPokemon(Pokemon newMon, Boolean isTrainerAboutToUse = false)
    {
        if (!_playerUnit.Mon.IsFainted)
        {
            yield return _dialogBox.TypeDialog($"Come back {_playerUnit.Mon.Name}!");

            _playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        _playerUnit.Setup(newMon);
        _dialogBox.SetMoveNames(newMon.Moves);
        yield return _dialogBox.TypeDialog($"Go {newMon.Name}!");
        
        if (isTrainerAboutToUse)
            StartCoroutine(SendNextTrainerPokemon());
        else
            _state = BattleState.RunningTurn;
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        Boolean canRunMove = sourceUnit.Mon.OnBeforeMove();
        yield return ShowStatusChanges(sourceUnit.Mon);
        
        if (!canRunMove)
        {
            yield return sourceUnit.HUD.UpdateHP();
            yield break;
        }
        
        move.CurrentPP--;
        yield return _dialogBox.TypeDialog($"{sourceUnit.Mon.Name} used {move.Name}.");

        if (CheckIfMoveHits(move, sourceUnit.Mon, targetUnit.Mon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);

            targetUnit.PlayHitAnimation();

            if (move.IsStatus)
            {
                yield return RunMoveEffects(move.Effects, sourceUnit.Mon, targetUnit.Mon, move.Target);
            }
            else
            {
                DamageDetails damageDetails = targetUnit.Mon.TakeDamage(move, sourceUnit.Mon);
                yield return targetUnit.HUD.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.SecondaryEffects is { Count: > 0 } && !targetUnit.Mon.IsFainted)
            {
                foreach (SecondaryEffects secondary in move.SecondaryEffects)
                {
                    Int32 rnd = URandom.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Mon, targetUnit.Mon, secondary.Target);
                }
            }

            if (targetUnit.Mon.IsFainted)
                yield return HandlePokemonFainted(targetUnit);
        }
        else
        {
            yield return _dialogBox.TypeDialog($"{targetUnit.Mon.Name}'s attack missed!");
        }

        yield return RunAfterTurn(sourceUnit);
    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (_state == BattleState.BattleOver)
            yield break;

        yield return new WaitUntil(() => _state == BattleState.RunningTurn);
        
        sourceUnit.Mon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Mon);
        yield return sourceUnit.HUD.UpdateHP();
        
        if (sourceUnit.Mon.IsFainted)
        {
            yield return HandlePokemonFainted(sourceUnit);
            yield return new WaitUntil(() => _state == BattleState.RunningTurn);
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
            if (!_isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                Pokemon nextMon = _trainerParty.GetHealthyPokemon();
                if (nextMon != null)
                    StartCoroutine(AboutToUse(nextMon));
                else
                    BattleOver(true);
            }
        }
    }

    private IEnumerator ShowStatusChanges(Pokemon mon)
    {
        while (mon.StatusChanges.Count > 0)
            yield return _dialogBox.TypeDialog(mon.StatusChanges.Dequeue());
    }
    
    private IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {
        if (effects.StatBoosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoosts(effects.StatBoosts);
            else
                target.ApplyBoosts(effects.StatBoosts);
        }

        if (effects.Status != ConditionID.None)
            target.SetStatus(effects.Status);
        if (effects.VolatileStatus != ConditionID.None)
            target.SetVolatileStatus(effects.VolatileStatus);

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    private Boolean CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.AlwaysHits)
            return true;
        
        Single moveAccuracy = move.Accuracy;
        Int32 accuracy = source.StatBoosts[Stat.Accuracy];
        Int32 evasion = target.StatBoosts[Stat.Evasion];

        Single[] boostValues = { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else if (accuracy < 0)
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else if (evasion < 0)
            moveAccuracy *= boostValues[-evasion];

        return URandom.Range(1, 101) <= moveAccuracy;
    }

    private IEnumerator RunTurns(BattleAction playerAction)
    {
        _state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            _playerUnit.Mon.CurrentMove = _playerUnit.Mon.Moves[_currentMove];
            _enemyUnit.Mon.CurrentMove = _enemyUnit.Mon.GetRandomMove();

            Int32 playerPriority = _playerUnit.Mon.CurrentMove.Priority;
            Int32 enemyPriority = _enemyUnit.Mon.CurrentMove.Priority;

            Boolean playerGoesFirst = true;
            if (enemyPriority > playerPriority)
                playerGoesFirst = false;
            else if (enemyPriority == playerPriority)
                playerGoesFirst = _playerUnit.Mon.Speed >= _enemyUnit.Mon.Speed;            
            
            BattleUnit firstUnit = playerGoesFirst ? _playerUnit : _enemyUnit;
            BattleUnit secondUnit = playerGoesFirst ? _enemyUnit : _playerUnit;
            Pokemon secondMon = secondUnit.Mon;

            yield return RunMove(firstUnit, secondUnit, firstUnit.Mon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            
            if (_state == BattleState.BattleOver)
                yield break;
            
            if (!secondMon.IsFainted)
            {
                yield return RunMove(secondUnit, firstUnit, secondUnit.Mon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchMon)
            {
                Pokemon selectedMon = _partyScreen.SelectedMember;
                _state = BattleState.Busy;
                yield return SwitchPokemon(selectedMon);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                _dialogBox.EnableActionSelector(false);
                yield return ThrowPokeball();
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }

            yield return RunMove(_enemyUnit, _playerUnit, _enemyUnit.Mon.GetRandomMove());
            yield return RunAfterTurn(_enemyUnit);
        }

        if (_state != BattleState.BattleOver)
            ActionSelection();
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        _playerParty = playerParty;
        _trainerParty = trainerParty;
        _isTrainerBattle = true;

        _playerController = _playerParty.GetComponent<PlayerController>();
        _trainerController = _trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    private IEnumerator SendNextTrainerPokemon()
    {
        _state = BattleState.Busy;

        _enemyUnit.Setup(_trainerParty.GetHealthyPokemon());
        yield return _dialogBox.TypeDialog($"{_trainerController.Name} sent out {_enemyUnit.Mon.Name}");

        _state = BattleState.RunningTurn;
    }

    private IEnumerator AboutToUse(Pokemon newMon)
    {
        _state = BattleState.Busy;
        
        yield return _dialogBox.TypeDialog($"{_trainerController.Name} is about to use {newMon.Name}. Do you want to change Pokemon?");

        _state = BattleState.AboutToUse;
        _dialogBox.EnableChoiceBox(true);
    }

    private IEnumerator ThrowPokeball()
    {
        if (_isTrainerBattle)
        {
            yield return _dialogBox.TypeDialog("You can't steal another trainer's Pokemon!");

            _state = BattleState.RunningTurn;
            yield break;
        }
        
        _state = BattleState.Busy;
        yield return _dialogBox.TypeDialog($"{_playerController.Name} used a Pokeball!");

        GameObject pokeballObj = Instantiate(_pokeballPrefab, _playerUnit.transform.position - new Vector3(2f, 0f), Quaternion.identity);
        SpriteRenderer pokeball = pokeballObj.GetComponent<SpriteRenderer>();

        Vector3 position = _enemyUnit.transform.position;
        yield return pokeball.transform.DOJump(
            position + new Vector3(0f, 2f), 2f, 1, 1f).WaitForCompletion();
        yield return _enemyUnit.PlayCaptureAnimation();
        yield return pokeball.transform.DOMoveY(position.y - 0.6f, 0.5f).WaitForCompletion();

        Int32 shakeCount = TryToCatchPokemon(_enemyUnit.Mon);

        for (Int32 i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0f, 0f, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            yield return _dialogBox.TypeDialog($"{_enemyUnit.Mon.Name} was caught!");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();

            _playerParty.AddPokemon(_enemyUnit.Mon);
            yield return _dialogBox.TypeDialog($"{_enemyUnit.Mon.Name} was added to the party");
            
            Destroy(pokeball);
            BattleOver(true);
        }
        else
        {
            yield return new WaitForSeconds(1f);

            pokeball.DOFade(0, 0.2f);
            yield return _enemyUnit.PlayBreakOutAnimation();
            
            if (shakeCount < 2)
                yield return _dialogBox.TypeDialog($"{_enemyUnit.Mon.Name} broke free!");
            else
                yield return _dialogBox.TypeDialog($"{_enemyUnit.Mon.Name} was almost caught!");

            Destroy(pokeball);
            _state = BattleState.RunningTurn;
        }
    }

    private Int32 TryToCatchPokemon(Pokemon mon)
    {
        Single a = (3 * mon.MaxHP - 2 * mon.CurrentHP) * mon.CatchRate * ConditionsDB.GetStatusBonus(mon.Status) / (3 * mon.MaxHP);
        if (a >= 255)
            return 4;

        Single b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));
        Int32 shakeCount = 0;
        
        while (shakeCount < 4)
        {
            if (URandom.Range(0, 65535) >= b)
                break;

            shakeCount++;
        }

        return shakeCount;
    }

    private IEnumerator TryToEscape()
    {
        _state = BattleState.Busy;

        if (_isTrainerBattle)
        {
            yield return _dialogBox.TypeDialog("You can't run from a trainer battle!");

            _state = BattleState.RunningTurn;
            yield break;
        }

        _escapeAttempts++;

        Int32 playerSpeed = _playerUnit.Mon.Speed;
        Int32 enemySpeed = _enemyUnit.Mon.Speed;

        if (enemySpeed < playerSpeed)
        {
            yield return _dialogBox.TypeDialog("Escaped successfully!");
            BattleOver(true);
        }
        else
        {
            Single f = (playerSpeed * 128) / enemySpeed + 30 * _escapeAttempts;
            f %= 256;

            if (URandom.Range(0, 256) < f)
            {
                yield return _dialogBox.TypeDialog("Escaped successfully!");
                BattleOver(true);
            }
            else
            {
                yield return _dialogBox.TypeDialog("Couldn't escape!");
                _state = BattleState.RunningTurn;
            }
        }
    }

    private IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return _dialogBox.TypeDialog($"{faintedUnit.Mon.Name} fainted");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsPlayer)
        {
            Int32 expAmt = faintedUnit.Mon.EXPYield;
            Int32 faintedLevel = faintedUnit.Mon.Level;
            Single trainerBonus = _isTrainerBattle ? 1.5f : 1f;

            Int32 expGain = Mathf.FloorToInt(expAmt * faintedLevel * trainerBonus / 7);
            _playerUnit.Mon.EXP += expGain;
            yield return _dialogBox.TypeDialog($"{_playerUnit.Mon.Name} gained {expGain} exp");

            yield return _playerUnit.HUD.SetEXPSmooth();

            while (_playerUnit.Mon.CheckForLevelUp())
            {
                _playerUnit.HUD.SetLevel();
                yield return _dialogBox.TypeDialog($"{_playerUnit.Mon.Name} grew to level {_playerUnit.Mon.Level}!");

                LearnableMove newMove = _playerUnit.Mon.GetLearnableMoveAtCurrentLevel();
                if (newMove != null)
                {
                    if (_playerUnit.Mon.Moves.Count < PokemonBase.MaxNumberOfMoves)
                    {
                        _playerUnit.Mon.LearnMove(newMove);
                        yield return _dialogBox.TypeDialog($"{_playerUnit.Mon.Name} learned {newMove.Base.Name}");
                        _dialogBox.SetMoveNames(_playerUnit.Mon.Moves);
                    }
                    else
                    {
                        yield return _dialogBox.TypeDialog($"{_playerUnit.Mon.Name} is trying to learn {newMove.Base.Name}...");
                        yield return _dialogBox.TypeDialog($"But it can't learn more than {PokemonBase.MaxNumberOfMoves} moves");

                        yield return ChooseMoveToForget(_playerUnit.Mon, newMove.Base);
                        
                        yield return new WaitUntil(() => _state != BattleState.MoveToForget);
                        yield return new WaitForSeconds(2f);
                    }
                }
                
                yield return _playerUnit.HUD.SetEXPSmooth(true);
            }

            yield return new WaitForSeconds(1f);
        }

        CheckForBattleOver(faintedUnit);
    }

    private IEnumerator ChooseMoveToForget(Pokemon mon, MoveBase newMove)
    {
        _state = BattleState.Busy;
        yield return _dialogBox.TypeDialog("Choose a move you want to forget...");

        _moveSelectionUI.gameObject.SetActive(true);
        _moveSelectionUI.SetMoveData(mon.Moves.Select(m=>m.Base).ToList(), newMove);
        _moveToLearn = newMove;

        _state = BattleState.MoveToForget;
    }
}