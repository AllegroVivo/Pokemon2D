using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private Text _messageText;
    
    private PartyMemberUI[] _memberSlots;
    private List<Pokemon> _partyList;
    
    private Int32 _selection = 0;

    public Pokemon SelectedMember => _partyList[_selection];
    
    public BattleState? CalledFrom { get; set; }

    public void Init()
    {
        _memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
    }

    public void SetPartyData(List<Pokemon> monList)
    {
        _partyList = monList;
        
        for (Int32 i = 0; i < _memberSlots.Length; i++)
        {
            if (i < monList.Count)
            {
                _memberSlots[i].gameObject.SetActive(true);
                _memberSlots[i].SetData(monList[i]);
            }
            else
            {
                _memberSlots[i].gameObject.SetActive(false);
            }
        }

        UpdateMemberSelection(_selection);
        _messageText.text = "Choose a Pokemon...";
    }

    public void UpdateMemberSelection(Int32 selectedMember)
    {
        for (Int32 i = 0; i < _partyList.Count; i++)
            _memberSlots[i].SetSelected(i == selectedMember);
    }

    public void SetMessageText(String message) => _messageText.text = message;
    
    public void HandleUpdate(Action onSelected, Action onBack)
    {
        Int32 prevSelection = _selection;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            _selection++;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            _selection--;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            _selection += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            _selection -= 2;

        _selection = Mathf.Clamp(_selection, 0, _partyList.Count - 1);

        if (_selection != prevSelection)
            UpdateMemberSelection(_selection);

        if (Input.GetKeyDown(KeyCode.Z))
            onSelected?.Invoke();
        else if (Input.GetKeyDown(KeyCode.X))
            onBack?.Invoke();
    }
}
