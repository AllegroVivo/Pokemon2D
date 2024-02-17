using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    [SerializeField] private Text _messageText;
    
    private PartyMemberUI[] _memberSlots;

    public void Init()
    {
        _memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Pokemon> monList)
    {
        for (Int32 i = 0; i < _memberSlots.Length; i++)
        {
            if (i < monList.Count)
                _memberSlots[i].SetData(monList[i]);
            else
                _memberSlots[i].gameObject.SetActive(false);
        }

        _messageText.text = "Choose a Pokemon...";
    }
}
