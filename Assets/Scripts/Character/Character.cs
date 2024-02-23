using System;
using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Single moveSpeed;
    
    private CharacterAnimator _animator;
    
    public Boolean IsMoving { get; private set; }
    
    public Single OffsetY => 0.3f;

    public CharacterAnimator Animator => _animator;

    private void Awake()
    {
        _animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnapToTile(transform.position);
    }

    public IEnumerator Move(Vector2 moveVector, Action onMoveOver = null)
    {
        _animator.MoveX = Mathf.Clamp(moveVector.x, -1, 1);
        _animator.MoveY = Mathf.Clamp(moveVector.y, -1, 1);
                
        Vector3 targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;
        
        if (!IsPathClear(targetPos))
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

    private Boolean IsPathClear(Vector3 targetPos)
    {
        Vector3 position = transform.position;
        Vector3 diff = targetPos - position;
        Vector3 dir = diff.normalized;
        return Physics2D.BoxCast(position + dir, new Vector2(0.2f, 0.2f), 0f, dir, diff.magnitude - 1,
            GameLayers.I.SolidObjectsLayer | GameLayers.I.InteractableLayer | GameLayers.I.PlayerLayer) != true;
    }

    public void LookTowards(Vector3 targetPos)
    {
        Vector3 position = transform.position;
        Single xDiff = Mathf.Floor(targetPos.x) - Mathf.Floor(position.x);
        Single yDiff = Mathf.Floor(targetPos.y) - Mathf.Floor(position.y);

        if (xDiff == 0 || yDiff == 0)
        {
            _animator.MoveX = Mathf.Clamp(xDiff, -1f, 1f);
            _animator.MoveY = Mathf.Clamp(yDiff, -1f, 1f);
        }
        else
        {
            Debug.LogError("Error in LookTowards: Character may not look diagonally.");
        }
    }

    public void SetPositionAndSnapToTile(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;

        transform.position = pos;
    }
}