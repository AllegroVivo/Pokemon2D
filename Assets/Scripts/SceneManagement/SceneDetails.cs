using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDetails : MonoBehaviour
{
    [SerializeField] private List<SceneDetails> _connectedScenes;
    
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

            if (GameController.I.PreviousScene != null)
            {
                List<SceneDetails> previouslyLoadedScenes = GameController.I.PreviousScene.ConnectedScenes;
                foreach (SceneDetails scene in previouslyLoadedScenes)
                {
                    if (!ConnectedScenes.Contains(scene) && scene != this)
                        scene.UnloadScene();
                }
            }
        }
    }

    public void LoadScene()
    {
        if (!IsLoaded)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            IsLoaded = true;
        }
    }

    public void UnloadScene()
    {
        if (IsLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            IsLoaded = false;
        }
    }
}