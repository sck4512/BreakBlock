using UnityEngine;
using MJ.Data;
using UnityEngine.Events;

public class GameOverTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent onGameOver;
    [SerializeField] private Collider2D myCollider;

    private void OnTriggerEnter2D(Collider2D _Other)
    {
        Debug.Log(1);
        if(_Other.CompareTag(Tags.BlockTag))
        {
            Debug.Log(2);
            onGameOver.Invoke();
            Debug.Log(3);
            myCollider.enabled = false;
            Debug.Log(4);
        }
    }
}
