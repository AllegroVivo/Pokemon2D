using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] private List<Text> _moveTexts;
    [SerializeField] private Color _highlightColor;

    private Int32 _current = 0;    
    
    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (Int32 i = 0; i < currentMoves.Count; i++)
            _moveTexts[i].text = currentMoves[i].Name;

        _moveTexts[currentMoves.Count].text = newMove.Name;
    }

    public void HandleMoveSelection(Action<Int32> onSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            _current++;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            _current--;

        _current = Mathf.Clamp(_current, 0, PokemonBase.MaxNumberOfMoves);
        UpdateMoveSelection(_current);

        if (Input.GetKeyDown(KeyCode.Z))
            onSelected?.Invoke(_current);
    }

    public void UpdateMoveSelection(Int32 selection)
    {
        for (Int32 i = 0; i < PokemonBase.MaxNumberOfMoves + 1; i++)
            _moveTexts[i].color = i == selection ? _highlightColor : Color.black;
    }
}