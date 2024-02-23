using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public IEnumerator FadeIn(Single fadeTime)
    {
        yield return _image.DOFade(1f, fadeTime).WaitForCompletion();
    }

    public IEnumerator FadeOut(Single fadeTime)
    {
        yield return _image.DOFade(0f, fadeTime).WaitForCompletion();
    }
}