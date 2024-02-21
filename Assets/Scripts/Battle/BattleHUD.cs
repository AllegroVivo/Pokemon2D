using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _statusText;
    [SerializeField] private HPBar _hpBar;
    [SerializeField] private GameObject _expBar;
    
    [SerializeField] private Color _poisonColor;
    [SerializeField] private Color _burnColor;
    [SerializeField] private Color _sleepColor;
    [SerializeField] private Color _paralyzeColor;
    [SerializeField] private Color _freezeColor;

    private Pokemon _mon;

    private Dictionary<ConditionID, Color> _statusColors;
    
    public void SetData(Pokemon mon)
    {
        _mon = mon;
        
        _nameText.text = mon.Name;
        SetLevel();
        
        _hpBar.SetHP((Single)mon.CurrentHP / mon.MaxHP);
        SetEXP();

        _statusColors = new Dictionary<ConditionID, Color>
        {
            { ConditionID.Poison, _poisonColor },
            { ConditionID.Burn, _burnColor },
            { ConditionID.Sleep, _sleepColor },
            { ConditionID.Paralyze, _paralyzeColor },
            { ConditionID.Freeze, _freezeColor }
        };
        
        SetStatusText();
        _mon.OnStatusChanged += SetStatusText;
    }

    public void SetStatusText()
    {
        _statusText.text = _mon.Status == null ? String.Empty : _mon.Status.ShortName;

        if (_mon.Status != null)
            _statusText.color = _statusColors[_mon.Status!.ID];
    }

    public IEnumerator UpdateHP()
    {
        if (_mon.HPChanged)
        {
            yield return _hpBar.SetHPSmooth((Single)_mon.CurrentHP / _mon.MaxHP);
            _mon.HPChanged = false;
        }
    }

    public void SetEXP()
    {
        if (_expBar == null)
            return;

        _expBar.transform.localScale = new Vector3(GetNormalizedEXP(), 1f, 1f);
    }

    private Single GetNormalizedEXP()
    {
        Int32 currLevelExp = _mon.Base.GetEXPForLevel(_mon.Level);
        Int32 nextLevelExp = _mon.Base.GetEXPForLevel(_mon.Level + 1);
        return Mathf.Clamp01((Single)(_mon.EXP - currLevelExp) / (nextLevelExp - currLevelExp));
    }

    public IEnumerator SetEXPSmooth(Boolean reset = false)
    {
        if (_expBar == null)
            yield break;

        if (reset)
            _expBar.transform.localScale = new Vector3(0f, 0f, 0f);
        yield return _expBar.transform.DOScaleX(GetNormalizedEXP(), 1.5f).WaitForCompletion();
    }

    public void SetLevel() => _levelText.text = "Lvl " + _mon.Level;
}