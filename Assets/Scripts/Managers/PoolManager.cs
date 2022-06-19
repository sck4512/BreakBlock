
namespace MJ.Manager
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum ParticleType
    {
        Red, Blue, Green
    }
    public class PoolManager : MonoBehaviour
    {
        private static GameObject blockPrefab;
        private static GameObject greenOrbPrefab;
        private static GameObject ballPrefab;

        private static List<Block> blocks;
        private static List<GameObject> greenOrbs;
        private static List<Ball> balls;


        private static Dictionary<ParticleType, List<GameObject>> particles;
        private static GameObject redParticlePrefab;
        private static GameObject blueParticlePrefab;
        private static GameObject greenParticlePrefab;

        private static GameObject greenBallPrefab;
        private static List<GreenBall> greenBalls;

        static PoolManager()
        {
            blockPrefab = Resources.Load<GameObject>("Prefabs/Block");
            blocks = new List<Block>();

            greenOrbPrefab = Resources.Load<GameObject>("Prefabs/GreenOrb");
            greenOrbs = new List<GameObject>();

            ballPrefab = Resources.Load<GameObject>("Prefabs/Ball");
            balls = new List<Ball>();

            redParticlePrefab = Resources.Load<GameObject>("Prefabs/ParticleRed");       
            blueParticlePrefab = Resources.Load<GameObject>("Prefabs/ParticleBlue"); 
            greenParticlePrefab = Resources.Load<GameObject>("Prefabs/ParticleGreen");
            particles = new Dictionary<ParticleType, List<GameObject>>();
            particles.Add(ParticleType.Red, new List<GameObject>());
            particles.Add(ParticleType.Blue, new List<GameObject>());
            particles.Add(ParticleType.Green, new List<GameObject>());

            greenBallPrefab = Resources.Load<GameObject>("Prefabs/GreenBall");
            greenBalls = new List<GreenBall>();
        }
        public static void Init()
        {
            blocks.Clear();
            greenOrbs.Clear();
            foreach (var particleContainer in particles)
            {
                particleContainer.Value.Clear();
            }

            greenBalls.Clear();
        }

        public static Block GetBlock()
        {
            foreach (var block in blocks)
            {
                if(!block.gameObject.activeSelf)
                {
                    return block;
                }
            }

            var blockComponent = Instantiate(blockPrefab).GetComponent<Block>();
            blocks.Add(blockComponent);
            return blockComponent;
        }

        public static Ball GetBall()
        {
            foreach (var ball in balls)
            {
                if (!ball.gameObject.activeSelf)
                {
                    return ball;
                }
            }

            var ballComponent = Instantiate(ballPrefab).GetComponent<Ball>();
            balls.Add(ballComponent);
            return ballComponent;
        }




        public static GameObject GetGreenOrb()
        {
            foreach (var greenOrb in greenOrbs)
            {
                if (!greenOrb.activeSelf)
                {
                    return greenOrb;
                }
            }

            var greenOrbObj = Instantiate(greenOrbPrefab);
            greenOrbs.Add(greenOrbObj);
            return greenOrbObj;
        }

        public static GreenBall GetGreenBall()
        {
            foreach (var greenBall in greenBalls)
            {
                if (!greenBall.gameObject.activeSelf)
                {
                    return greenBall;
                }
            }

            var greenBallComponent = Instantiate(greenBallPrefab).GetComponent<GreenBall>();
            greenBalls.Add(greenBallComponent);
            return greenBallComponent;
        }



        public static GameObject GetParticle(ParticleType _Type)
        {
            foreach (var particle in particles[_Type])
            {
                if(!particle.activeSelf)
                {
                    return particle;
                }
            }

            GameObject particleObj = null;
            switch (_Type)
            {
                case ParticleType.Red:
                    particleObj = Instantiate(redParticlePrefab);
                    break;
                case ParticleType.Blue:
                    particleObj = Instantiate(blueParticlePrefab);
                    break;
                case ParticleType.Green:
                    particleObj = Instantiate(greenParticlePrefab);
                    break;
            }

            particles[_Type].Add(particleObj);
            return particleObj;
        }
    }
}
