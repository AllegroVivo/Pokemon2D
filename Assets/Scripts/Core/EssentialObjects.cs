using System;
using UnityEngine;

public class EssentialObjects : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}