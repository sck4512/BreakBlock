using UnityEngine;
using System.Collections;
using System;
using MJ.Data;

[DisallowMultipleComponent]
public sealed class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;

    private static readonly float ShootForce = 7000f;
    private Transform myTransform;
    public bool IsMoving { get; set; } = false;
    public event Action<Vector3> OnTouchBottom;

    private void Awake()
    {
        myTransform = transform;
    }


    public void Shoot(Vector3 _Direction)
    {
        IsMoving = true;
        rigidBody.AddForce(_Direction * ShootForce);
    }

    private void OnCollisionEnter2D(Collision2D _Other)
    {
        if(_Other.gameObject.CompareTag(Tags.BottomTag))
        {
            rigidBody.velocity = Vector2.zero;
            var ballPos = new Vector3(_Other.contacts[0].point.x, Constants.BottomY, myTransform.position.z);
            myTransform.position = ballPos; 
    
            GameManager.Instance.SetTotalBallPos(ballPos);

            StartCoroutine(MoveToTotalBallPos());
        }
    }

    private IEnumerator MoveToTotalBallPos()
    {
        var targetPos = GameManager.Instance.TotalBallPos;
        while (myTransform.position != targetPos)
        {
            myTransform.position = Vector3.MoveTowards(myTransform.position, targetPos, 4f);
            yield return null;
        }
        IsMoving = false;
    }
}
