using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private List<Sprite> _walkUpSprites;
    [SerializeField] private List<Sprite> _walkDownSprites;
    [SerializeField] private List<Sprite> _walkLeftSprites;
    [SerializeField] private List<Sprite> _walkRightSprites;

    [SerializeField] private FacingDirection _defaultDirection = FacingDirection.Down;
    
    public Single MoveX { get; set; }
    public Single MoveY { get; set; }
    public Boolean IsMoving { get; set; }

    private Boolean _wasMoving;

    private SpriteAnimator _walkUpAnim;
    private SpriteAnimator _walkDownAnim;
    private SpriteAnimator _walkLeftAnim;
    private SpriteAnimator _walkRightAnim;

    private SpriteAnimator _currentAnim;

    public FacingDirection DefaultDirection => _defaultDirection;

    private SpriteRenderer _spriteRenderer;
    
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _walkUpAnim = new SpriteAnimator(_walkUpSprites, _spriteRenderer);
        _walkDownAnim = new SpriteAnimator(_walkDownSprites, _spriteRenderer);
        _walkLeftAnim = new SpriteAnimator(_walkLeftSprites, _spriteRenderer);
        _walkRightAnim = new SpriteAnimator(_walkRightSprites, _spriteRenderer);

        SetFacingDirection(_defaultDirection);

        _currentAnim = _walkDownAnim;
        
    }

    private void Update()
    {
        SpriteAnimator prevAnim = _currentAnim;
        
        if (MoveX == 1)
            _currentAnim = _walkRightAnim;
        else if (MoveX == -1)
            _currentAnim = _walkLeftAnim;
        else if (MoveY == 1)
            _currentAnim = _walkUpAnim;
        else if (MoveY == -1)
            _currentAnim = _walkDownAnim;
        
        if (_currentAnim != prevAnim || IsMoving != _wasMoving)
            _currentAnim.Start();

        if (IsMoving)
            _currentAnim.HandleUpdate();
        else
            _spriteRenderer.sprite = _currentAnim.Frames[0];

        _wasMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection dir)
    {
        if (dir == FacingDirection.Right)
            MoveX = 1;
        else if (dir == FacingDirection.Left)
            MoveX = -1;
        else if (dir == FacingDirection.Down)
            MoveY = -1;
        else if (dir == FacingDirection.Up)
            MoveY = 1;
    }
}