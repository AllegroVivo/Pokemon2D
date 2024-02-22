using System;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] private LayerMask _solidObjects;
    [SerializeField] private LayerMask _longGrass;
    [SerializeField] private LayerMask _interactable;
    [SerializeField] private LayerMask _player;
    [SerializeField] private LayerMask _fov;
    [SerializeField] private LayerMask _portal;

    public LayerMask SolidObjectsLayer => _solidObjects;
    public LayerMask GrassLayer => _longGrass;
    public LayerMask InteractableLayer => _interactable;
    public LayerMask PlayerLayer => _player;
    public LayerMask FoVLayer => _fov;
    public LayerMask PortalLayer => _portal;

    public LayerMask TriggerableLayers => _longGrass | _fov | _portal;
    
    public static GameLayers I { get; private set; }

    private void Awake()
    {
        I = this;
    }
}