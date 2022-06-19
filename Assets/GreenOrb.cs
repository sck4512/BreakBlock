using UnityEngine;
using MJ.Data;
using MJ.Manager;

public class GreenOrb : Movable
{
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
    }
}
