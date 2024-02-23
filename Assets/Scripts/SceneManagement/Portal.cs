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
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        GameController.I.PauseGame(true);
        yield return _fader.FadeIn(0.5f);
        
        yield return SceneManager.LoadSceneAsync(_sceneToLoad);

        Portal destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.Destination == _destination);
        _player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        yield return _fader.FadeOut(0.5f);
        GameController.I.PauseGame(false);

        Destroy(gameObject);
    }
}