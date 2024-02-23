using System;
using System.Collections;
using UnityEngine;
using Object = System.Object;
using URandom = UnityEngine.Random;

public class PlayerController : MonoBehaviour, ISavable
{
    [SerializeField] private String _name;
    [SerializeField] private Sprite _sprite;
    
    private Vector2 _input;
    private Character _character;
    
    public Sprite Sprite => _sprite;
    public String Name => _name;
    public Character Character => _character;

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
                StartCoroutine(_character.Move(_input, OnMoveOver));
        }

        _character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    private void Interact()
    {
        Vector3 facingDir = new(_character.Animator.MoveX, _character.Animator.MoveY);
        Vector3 interactPos = transform.position + facingDir;

        Collider2D coll = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.I.InteractableLayer);
        if (coll != null)
            coll.GetComponent<IInteractable>()?.Interact(transform);
    }

    private void OnMoveOver()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position - new Vector3(0f, _character.OffsetY), 0.2f, GameLayers.I.TriggerableLayers);

        foreach (Collider2D coll in colliders)
        {
            IPlayerTriggerable triggerable = coll.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    public Object CaptureState()
    {
        Vector3 pos = transform.position;
        return new[] { pos.x, pos.y };
    }

    public void RestoreState(Object state)
    {
        Single[] position = (Single[])state;
        transform.position = new Vector3(position[0], position[1]);
    }
}
