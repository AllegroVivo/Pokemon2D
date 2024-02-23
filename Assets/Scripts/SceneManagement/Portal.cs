using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Int32 _sceneToLoad = -1;
    [SerializeField] private Transform _spawnPoint;

    private PlayerController _player;

    public Transform SpawnPoint => _spawnPoint;
    
    public void OnPlayerTriggered(PlayerController player)
    {
        _player = player;
        StartCoroutine(SwitchScene());
    }

    private IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);
        
        yield return SceneManager.LoadSceneAsync(_sceneToLoad);

        Portal destPortal = FindObjectsOfType<Portal>().First(x => x != this);
        _player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);

        Destroy(gameObject);
    }
}