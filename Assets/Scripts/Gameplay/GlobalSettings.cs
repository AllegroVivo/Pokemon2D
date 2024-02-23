using System;
using UnityEngine;

public class GlobalSettings : MonoBehaviour
{
    [SerializeField] private Color _highlightColor;

    public Color HighlightColor => _highlightColor;
    
    public static GlobalSettings I { get; private set; }

    private void Awake()
    {
        I = this;
    }
}