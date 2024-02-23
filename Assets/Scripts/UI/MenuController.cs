using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject _menu;

    private List<Text> _menuItems;
    private Int32 _selection = 0;

    public event Action<Int32> OnMenuSelected;
    public event Action OnBack;

    private void Awake()
    {
        _menuItems = _menu.GetComponentsInChildren<Text>().ToList();
    }

    public void OpenMenu()
    {
        _menu.SetActive(true);
        UpdateItemSelection();
    }

    public void CloseMenu()
    {
        _menu.SetActive(false);
    }

    public void HandleUpdate()
    {
        Int32 prevSelection = _selection;
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
            _selection++;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            _selection--;

        _selection = Mathf.Clamp(_selection, 0, _menuItems.Count - 1);
        
        if (prevSelection != _selection)
            UpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnMenuSelected?.Invoke(_selection);
            CloseMenu();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            OnBack?.Invoke();
            CloseMenu();
        }
    }

    private void UpdateItemSelection()
    {
        for (Int32 i = 0; i < _menuItems.Count; i++)
            _menuItems[i].color = i==_selection ? GlobalSettings.I.HighlightColor : Color.black;
    }
}