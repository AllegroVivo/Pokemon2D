using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Single moveSpeed;
    
    private CharacterAnimator _animator;
    
    public Boolean IsMoving { get; private set; }
    public CharacterAnimator Animator => _animator;

    private void Awake()
    {
        _animator = GetComponent<CharacterAnimator>();
    }

    public IEnumerator Move(Vector2 moveVector, Action onMoveOver = null)
    {
        _animator.MoveX = Mathf.Clamp(moveVector.x, -1, 1);
        _animator.MoveY = Mathf.Clamp(moveVector.y, -1, 1);
                
        Vector3 targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;
        
        if (!IsWalkable(targetPos))
            yield break;
        
        IsMoving = true;
        
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        IsMoving = false;

        onMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        _animator.IsMoving = IsMoving;
    }
    
    private Boolean IsWalkable(Vector3 targetPos) 
        => Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.I.SolidObjectsLayer | GameLayers.I.InteractableLayer) == null;
}