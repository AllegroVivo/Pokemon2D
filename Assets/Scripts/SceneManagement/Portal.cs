using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Int32 _sceneToLoad = -1;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private DestinationID _destination;

    private PlayerController _player;

    public Transform SpawnPoint => _spawnPoint;
    public DestinationID Destination => _destination;
    
    public void OnPlayerTriggered(PlayerController player)
    {
        _player = player;
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        GameController.I.PauseGame(true);
        
        yield return SceneManager.LoadSceneAsync(_sceneToLoad);

        Portal destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.Destination == _destination);
        _player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        GameController.I.PauseGame(false);

        Destroy(gameObject);
    }
}