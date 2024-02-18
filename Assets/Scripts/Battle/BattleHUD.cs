using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _levelText;
    [SerializeField] private HPBar _hpBar;

    private Pokemon _mon;
    
    public void SetData(Pokemon mon)
    {
        _mon = mon;
        
        _nameText.text = mon.Name;
        _levelText.text = "Lvl " + mon.Level;
        _hpBar.SetHP((Single)mon.CurrentHP / mon.MaxHP);
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