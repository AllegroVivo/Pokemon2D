using System;
using System.Collections;
using UnityEngine;
using URandom = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public event Action OnEncountered;
    
    private Vector2 _input;
    private Character _character;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if (!_character.IsMoving)
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");

            if (_input.x != 0)
                _input.y = 0;

            if (_input != Vector2.zero)
            {
                StartCoroutine(_character.Move(_input, CheckForEncounters));
            }
        }
        
        _character.HandleUpdate();
        
        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.I.GrassLayer) != null)
        {
            if (URandom.Range(1, 101) <= 10)
            {
                _character.Animator.IsMoving = false;
                OnEncountered?.Invoke();
            }
        }
    }

    private void Interact()
    {
        Vector3 facingDir = new(_character.Animator.MoveX, _character.Animator.MoveY);
        Vector3 interactPos = transform.position + facingDir;

        Collider2D coll = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.I.InteractableLayer);
        if (coll != null)
            coll.GetComponent<IInteractable>()?.Interact(transform);
    }
}
