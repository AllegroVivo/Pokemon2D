using System;
using System.Collections;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] private String _name;
    [SerializeField] private Sprite _sprite;
    
    [SerializeField] private Dialog _dialog;
    [SerializeField] private GameObject _exclamation;
    [SerializeField] private GameObject _fov;

    public Sprite Sprite => _sprite;
    public String Name => _name;

    private Character _character;

    private void Awake()
    {
        _character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFoVRotation(_character.Animator.DefaultDirection);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        _exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        _exclamation.SetActive(false);

        Vector3 diff = player.transform.position - transform.position;
        Vector2 moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return _character.Move(moveVec);
        
        StartCoroutine(DialogManager.I.ShowDialog(_dialog, () =>
        {
            GameController.I.StartTrainerBattle(this);
        }));
    }

    public void SetFoVRotation(FacingDirection dir)
    {
        Single angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180;
        else if (dir == FacingDirection.Left)
            angle = 270f;

        _fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}