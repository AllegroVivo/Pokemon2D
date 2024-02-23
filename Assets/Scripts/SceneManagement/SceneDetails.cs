using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] private List<SceneDetails> _connectedScenes;

    private List<SavableEntity> _savableEntities;
    
    public Boolean IsLoaded { get; private set; }
    public List<SceneDetails> ConnectedScenes => _connectedScenes;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Entered scene: {gameObject.name}");

            LoadScene();
            GameController.I.SetCurrentScene(this);
            
            foreach(SceneDetails scene in _connectedScenes)
                scene.LoadScene();

            SceneDetails prevScene = GameController.I.PreviousScene;
            if (prevScene != null)
            {
                List<SceneDetails> previouslyLoadedScenes = prevScene.ConnectedScenes;
                foreach (SceneDetails scene in previouslyLoadedScenes)
                {
                    if (!ConnectedScenes.Contains(scene) && scene != this)
                        scene.UnloadScene();
                }

                if (!_connectedScenes.Contains(prevScene))
                    prevScene.UnloadScene();
            }
        }
    }

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;

            operation.completed += _ =>
            {
                _savableEntities = GetSavableEntitiesInScene();
                SavingSystem.I.RestoreEntityStates(_savableEntities);
            };
        }
    }

    private List<SavableEntity> GetSavableEntitiesInScene()
    {
        Scene currentScene = SceneManager.GetSceneByName(gameObject.name);
        return FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == currentScene).ToList();
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SavingSystem.I.CaptureEntityStates(_savableEntities);
            
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }
}