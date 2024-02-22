using System;
using UnityEngine;

public class TrainerFOV : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player) => GameController.I.OnEnterTrainerView(GetComponentInParent<TrainerController>());
}
