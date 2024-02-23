using System;
using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _essentialObjectsPrefab;

    private void Awake()
    {
        EssentialObjects[] existing = FindObjectsOfType<EssentialObjects>();
        if (existing.Length == 0)
            Instantiate(_essentialObjectsPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);
    }
}