using System;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialog _dialog;

    public void Interact()
    {
        StartCoroutine(DialogManager.I.ShowDialog(_dialog));
    }
}