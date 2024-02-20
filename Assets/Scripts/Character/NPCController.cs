using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialog _dialog;
    [SerializeField] private List<Vector2> _movePatterns;
    [SerializeField] private Single _patternTime;

    private NPCState _state;
    private Single _idleTimer = 0f;
    private Int32 _currentPattern = 0;

    private Character _character;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    public void Interact()
    {
        if (_state == NPCState.Idle)
            StartCoroutine(DialogManager.I.ShowDialog(_dialog));
    }

    private void Update()
    {
        if (DialogManager.I.IsShowing)
            return;
        
        if (_state == NPCState.Idle)
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer > _patternTime)
            {
                _idleTimer = 0f;
                if (_movePatterns.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        
        _character.HandleUpdate();
    }

    private IEnumerator Walk()
    {
        _state = NPCState.Walking;

        yield return _character.Move(_movePatterns[_currentPattern]);

        _currentPattern = (_currentPattern + 1) % _movePatterns.Count;

        _state = NPCState.Idle;
    }
}