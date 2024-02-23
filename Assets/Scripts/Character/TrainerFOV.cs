using System;
using UnityEngine;

public class TrainerFOV : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        GameController.I.OnEnterTrainerView(GetComponentInParent<TrainerController>());
    }
}
