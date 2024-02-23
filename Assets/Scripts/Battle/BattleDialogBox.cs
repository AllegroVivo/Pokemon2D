using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private Int32 _lettersPerSecond;
    [SerializeField] private Text _dialogText;
    
    [SerializeField] private GameObject _actionSelector;
    [SerializeField] private GameObject _moveSelector;
    [SerializeField] private GameObject _moveDetails;
    [SerializeField] private GameObject _choiceBox;

    [SerializeField] private List<Text> _actionTexts;
    [SerializeField] private List<Text> _moveTexts;

    [SerializeField] private Text _ppText;
    [SerializeField] private Text _typeText;

    [SerializeField] private Text _yesText;
    [SerializeField] private Text _noText;

    private Color _highlightColor;

    private void Start()
    {
        _highlightColor = GlobalSettings.I.HighlightColor;
    }

    public void SetDialog(String dialog)
    {
        _dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(String dialog)
    {
        _dialogText.text = String.Empty;

        foreach (Char c in dialog)
        {
            _dialogText.text += c;
            yield return new WaitForSeconds(1f / _lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(Boolean enable) => _dialogText.enabled = enable;
    public void EnableActionSelector(Boolean enable) => _actionSelector.SetActive(enable);
    public void EnableMoveSelector(Boolean enable)
    {
        _moveSelector.SetActive(enable);
        _moveDetails.SetActive(enable);
    }

    public void UpdateActionSelection(Int32 selectedAction)
    {
        for (Int32 i = 0; i < _actionTexts.Count; i++)
            _actionTexts[i].color = i == selectedAction ? _highlightColor : Color.black;
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (Int32 i = 0; i < _moveTexts.Count; i++)
            _moveTexts[i].text = i < moves.Count ? moves[i].Name : "-";
    }

    public void UpdateMoveSelection(Int32 selectedMove, Move move)
    {
        for (Int32 i = 0; i < _moveTexts.Count; i++)
            _moveTexts[i].color = i == selectedMove ? _highlightColor : Color.black;

        _ppText.text = $"PP {move.CurrentPP}/{move.MaxPP}";
        _ppText.color = move.CurrentPP <= 0 ? Color.red : Color.black;
        _typeText.text = move.Type.ToString();
    }

    public void EnableChoiceBox(Boolean enable) => _choiceBox.SetActive(enable);

    public void UpdateChoiceBoxSelection(Boolean yesSelected)
    {
        _yesText.color = yesSelected ? _highlightColor : Color.black;
        _noText.color = yesSelected ? Color.black : _highlightColor;
    }
}
