using System;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Interacting!");
    }
}