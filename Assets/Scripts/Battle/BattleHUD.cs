using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _levelText;
    [SerializeField] private Text _statusText;
    [SerializeField] private HPBar _hpBar;
    
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
        _levelText.text = "Lvl " + mon.Level;
        _hpBar.SetHP((Single)mon.CurrentHP / mon.MaxHP);

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
}