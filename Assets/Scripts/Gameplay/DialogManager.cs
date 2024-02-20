using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] private GameObject _dialogBox;
    [SerializeField] private Text _dialogText;
    [SerializeField] private Int32 _lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    private Int32 _currentLine = 0;
    private Dialog _currentDialog;
    private Boolean _isTyping;
    
    public Boolean IsShowing { get; private set; }
    
    public static DialogManager I { get; private set; }

    private void Awake()
    {
        I = this;
    }

    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        
        OnShowDialog?.Invoke();

        IsShowing = true;
        _currentDialog = dialog;
        _dialogBox.gameObject.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }
    
    public IEnumerator TypeDialog(String line)
    {
        _isTyping = true;
        
        _dialogText.text = String.Empty;
        foreach (Char c in line)
        {
            _dialogText.text += c;
            yield return new WaitForSeconds(1f / _lettersPerSecond);
        }

        _isTyping = false;
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !_isTyping)
        {
            _currentLine++;
            if (_currentLine < _currentDialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(_currentDialog.Lines[_currentLine]));
            }
            else
            {
                _currentLine = 0;
                IsShowing = false;
                _dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
            }
        }
    }
}