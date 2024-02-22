using System;
using UnityEngine;
using URandom = UnityEngine.Random;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        if (URandom.Range(1, 101) <= 10)
            GameController.I.StartBattle();
    }
}