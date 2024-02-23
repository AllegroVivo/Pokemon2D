using System;
using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _essentialObjectsPrefab;

    private void Awake()
    {
        EssentialObjects[] existing = FindObjectsOfType<EssentialObjects>();
        if (existing.Length == 0)
        {
            Vector3 spawnPos = new(0f, 0f, 0f);
            Grid grid = FindObjectOfType<Grid>();
            
            if (grid != null)
                spawnPos = grid.transform.position;
            
            Instantiate(_essentialObjectsPrefab, spawnPos, Quaternion.identity);
        }
    }
}