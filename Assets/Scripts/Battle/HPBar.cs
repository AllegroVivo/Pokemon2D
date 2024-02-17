using System;
using System.Collections;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] private GameObject _health;

    public void SetHP(Single hpNormalized) => _health.transform.localScale = new Vector3(hpNormalized, 1f);

    public IEnumerator SetHPSmooth(Single newHP)
    {
        Single currentHP = _health.transform.localScale.x;
        Single changeAmt = currentHP - newHP;

        while (currentHP - newHP > Mathf.Epsilon)
        {
            currentHP -= changeAmt * Time.deltaTime;
            _health.transform.localScale = new Vector3(currentHP, 1f);
            yield return null;
        }
        
        _health.transform.localScale  = new Vector3(newHP, 1f);
    }
}