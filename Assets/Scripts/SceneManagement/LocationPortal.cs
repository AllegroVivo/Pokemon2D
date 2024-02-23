using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationPortal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private DestinationID _destination;

    private PlayerController _player;
    private Fader _fader;

    public Transform SpawnPoint => _spawnPoint;
    public DestinationID Destination => _destination;

    private void Start()
    {
        _fader = FindObjectOfType<Fader>();
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        _player = player;
        player.Character.Animator.IsMoving = false;
        StartCoroutine(Teleport());
    }

    private IEnumerator Teleport()
    {
        GameController.I.PauseGame(true);
        yield return _fader.FadeIn(0.5f);
        
        LocationPortal destPortal = FindObjectsOfType<LocationPortal>().First(x => x != this && x.Destination == _destination);
        _player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return _fader.FadeOut(0.5f);
        GameController.I.PauseGame(false);
    }
}