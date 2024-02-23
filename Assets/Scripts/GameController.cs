using System;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene, Paused }

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private BattleSystem _battleSystem;
    [SerializeField] private Camera _worldCamera;
    
    private GameState _state;
    private GameState _prevState;
    private TrainerController _trainer;
    
    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PreviousScene { get; private set; }
    
    public static GameController I { get; private set; }

    private void Awake()
    {
        I = this;
        
        ConditionsDB.Init();
        PokemonDB.Init();
        MoveDB.Init();
    }

    private void Start()
    {
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
        {
            _playerController.HandleUpdate();
            
            if (Input.GetKeyDown(KeyCode.S))
                SavingSystem.I.Save("SaveSlot1");
            else if (Input.GetKeyDown(KeyCode.L))
                SavingSystem.I.Load("SaveSlot1");
        }
        else if (_state == GameState.Battle)
            _battleSystem.HandleUpdate();
        else if (_state == GameState.Dialog)
            DialogManager.I.HandleUpdate();
    }

    public void StartBattle()
    {
        _state = GameState.Battle;
        _battleSystem.gameObject.SetActive(true);
        _worldCamera.gameObject.SetActive(false);

        PokemonParty playerParty = _playerController.GetComponent<PokemonParty>();
        Pokemon wildMon = CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon();
        
        _battleSystem.StartBattle(playerParty, new Pokemon(wildMon.Base, wildMon.Level));
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        _state = GameState.Battle;
        _battleSystem.gameObject.SetActive(true);
        _worldCamera.gameObject.SetActive(false);

        _trainer = trainer;
        
        PokemonParty playerParty = _playerController.GetComponent<PokemonParty>();
        PokemonParty trainerParty = trainer.GetComponent<PokemonParty>();

        _battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    private void EndBattle(Boolean battleWon)
    {
        if (_trainer!=null && battleWon)
        {
            _trainer.BattleLost();
            _trainer = null;
        }
        
        _state = GameState.FreeRoam;
        _battleSystem.gameObject.SetActive(false);
        _worldCamera.gameObject.SetActive(true);
    }

    public void OnEnterTrainerView(TrainerController trainer)
    {
        _state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(_playerController));
    }

    public void PauseGame(Boolean pause)
    {
        if (pause)
        {
            _prevState = _state;
            _state = GameState.Paused;
        }
        else
        {
            _state = _prevState;
        }
    }

    public void SetCurrentScene(SceneDetails nextScene)
    {
        PreviousScene = CurrentScene;
        CurrentScene = nextScene;
    }
}