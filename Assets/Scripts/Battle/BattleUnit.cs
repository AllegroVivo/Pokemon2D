﻿using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private Int32 _level;
    [SerializeField] private Boolean _isPlayerUnit;

    public Pokemon Mon { get; set; }
    
    private Image _image;
    private Vector3 _originalPos;
    private Color _originalColor;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _originalPos = _image.transform.localPosition;
        _originalColor = _image.color;
    }

    public void Setup()
    {
        Mon = new Pokemon(_base, _level);
        _image.sprite = _isPlayerUnit ? Mon.BackSprite : Mon.FrontSprite;

        _image.color = _originalColor;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        _image.transform.localPosition = new Vector3(500f * (_isPlayerUnit ? -1f : 1f), _originalPos.y);
        _image.transform.DOLocalMoveX(_originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_image.transform.DOLocalMoveX(_originalPos.x + 50f * (_isPlayerUnit ? 1f : -1f), 0.25f));
        sequence.Append(_image.transform.DOLocalMoveX(_originalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_image.DOColor(Color.gray, 0.1f));
        sequence.Append(_image.DOColor(_originalColor, 0.1f));
        sequence.Append(_image.DOColor(Color.gray, 0.1f));
        sequence.Append(_image.DOColor(_originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_image.transform.DOLocalMoveY(_originalPos.y - 150f, 0.5f));
        sequence.Join(_image.DOFade(0f, 0.5f));
    }
}