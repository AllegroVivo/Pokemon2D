using System;
using System.Collections;
using UnityEngine;
using URandom = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public Single moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask longGrassLayer;
    
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

    private Boolean IsWalkable(Vector3 targetPos) => Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) == null;

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

        CheckForEncounters();
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, longGrassLayer) != null)
        {
            if (URandom.Range(1, 101) <= 10)
            {
                Debug.Log("Encountered a wild pokemon!");
            }
        }
    }
}
