using UnityEngine;
using MJ.Data;
using MJ.Manager;
using System.Collections;

public class GreenOrb : Movable
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void Suicide()
    {
        StartCoroutine(SuicideRoutine());
        IEnumerator SuicideRoutine()
        {
            while (spriteRenderer.color.a > 0f)
            {
                var color = spriteRenderer.color;
                color.a -= Time.deltaTime * 3.3f;
                spriteRenderer.color = color;
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter2D(Collider2D _Other)
    {
        if(_Other.CompareTag(Tags.BallTag))
        {
            SoundManager.PlayGreenOrbSound();

            var particle = PoolManager.GetParticle(ParticleType.Green);
            particle.SetActive(true);
            particle.transform.position = transform.position;

            var greenBall = PoolManager.GetGreenBall();
            greenBall.transform.position = transform.position;
            greenBall.gameObject.SetActive(true);
            greenBall.MoveToTarget(new Vector3(transform.position.x, Constants.BottomY, 0), 2.5f);
            GameManager.Instance.GreenBalls.Add(greenBall);

            gameObject.SetActive(false);
        }  
        else if(_Other.CompareTag(Tags.GameOverTriggerTag))
        {
            var particle = PoolManager.GetParticle(ParticleType.Green);
            particle.SetActive(true);
            particle.transform.position = transform.position;

            GameManager.Instance.AddBall();
            gameObject.SetActive(false);
        }
    }
}
