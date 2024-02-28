using System;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
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

    public void SetSelected(Boolean selected) => _nameText.color = selected ? GlobalSettings.I.HighlightColor : Color.black;
}