using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _levelText;
    [SerializeField] private HPBar _hpBar;

    public void SetData(Pokemon mon)
    {
        _nameText.text = mon.Name;
        _levelText.text = "Lvl " + mon.Level;
        _hpBar.SetHP((Single)mon.CurrentHP / mon.MaxHP);
    }
}