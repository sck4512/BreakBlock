using UnityEngine;
using TMPro;
using MJ.Data;
using System.Collections;
using System;
using MJ.Manager;


public sealed class Block : Movable
{
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshProUGUI numberText;
    public event Action OnDisableAction;
    private int leftTouchCount;
    private Color startColor;
    private int disableScore;


    protected override void Awake()
    {
        base.Awake();
        startColor = spriteRenderer.color;
    }

    private void OnEnable()
    {
        spriteRenderer.color = startColor;
    }

    public void SetNumber(int _Number)
    {
        leftTouchCount = _Number;
        numberText.text = leftTouchCount.ToString();

        disableScore = 1;
        if(5 < leftTouchCount)
        {
            disableScore++;
        }

        if (15 < leftTouchCount)
        {
            disableScore++;
        }

        if (30 < leftTouchCount)
        {
            disableScore++;
        }

        if (50 < leftTouchCount)
        {
            disableScore++;
        }
    }

    private void OnCollisionEnter2D(Collision2D _Other)
    {
        if(_Other.gameObject.CompareTag(Tags.BallTag))
        {
            leftTouchCount--;
            SoundManager.PlayBlockSound();


            if(leftTouchCount > 0)
            {
                ScoreManager.AddScore(1);
                //anim.SetTrigger("TakeDamage");
                //anim.Play("BlockTakeDamage");
                StopCoroutine("OnTakeDamageRoutine");
                StartCoroutine("OnTakeDamageRoutine");
                numberText.text = leftTouchCount.ToString();
            }
            else
            {
                var particle = PoolManager.GetParticle(ParticleType.Red);
                particle.transform.position = transform.position;
                particle.SetActive(true);

                ScoreManager.AddScore(disableScore);
                gameObject.SetActive(false);
            }
        }
    }    
    IEnumerator OnTakeDamageRoutine()
    {
        var targetColor = Color.white;
        //FB8061
        ColorUtility.TryParseHtmlString("FB8061", out targetColor);
        spriteRenderer.color = startColor;

        while (GetMagnitude(spriteRenderer.color, targetColor) > 0.01f)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, targetColor, 0.1f);
            yield return null;
        }
        spriteRenderer.color = startColor;
    }

    private float GetMagnitude(Color _ColorA, Color _ColorB)
    {
        float result = (_ColorA.r - _ColorB.r) * (_ColorA.r - _ColorB.r);
        result += (_ColorA.g - _ColorB.g) * (_ColorA.g - _ColorB.g);
        result += (_ColorA.b - _ColorB.b) * (_ColorA.b - _ColorB.b);
        return result;
    }
}
