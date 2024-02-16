using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Single moveSpeed;
    public LayerMask solidObjectsLayer;
    
    private Boolean isMoving;
    private Vector2 _input;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            _input.x = Input.GetAxisRaw("Horizontal");
            _input.y = Input.GetAxisRaw("Vertical");

            if (_input.x != 0)
                _input.y = 0;

            if (_input != Vector2.zero)
            {
                _animator.SetFloat("MoveX", _input.x);
                _animator.SetFloat("MoveY", _input.y);
                
                Vector2 targetPos = transform.position;
                targetPos.x += _input.x;
                targetPos.y += _input.y;

                if (IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }

        _animator.SetBool("IsMoving", isMoving);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }

    private Boolean IsWalkable(Vector3 targetPos) => Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) == null;
}
