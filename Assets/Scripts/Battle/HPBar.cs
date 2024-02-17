using System;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] private GameObject _health;

    public void SetHP(Single hpNormalized) => _health.transform.localScale = new Vector3(hpNormalized, 1f);
}