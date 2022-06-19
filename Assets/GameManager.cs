using MJ.Data;
using MJ.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnTransforms;
    [SerializeField] private InputController inputController;
    [SerializeField] private Transform arrowTransform;
    [SerializeField] private Transform ballPreviewTransform;
    [SerializeField] private LineRenderer ballShootLineRenderer;
    [SerializeField] private Ball startBall; //제일 처음 주는 공

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI bestScoreText;
    [SerializeField] private TextMeshProUGUI curScoreText;
    
    public static GameManager Instance => instance;
    private static GameManager instance;

    private int round;
    private Vector3 firstClickPos;
    private Vector3 secondClickPos;
    private Vector3 shootDirection;
    private float shootDirectionLength;
    private Vector3 totalBallPos;
    public Vector3 TotalBallPos => totalBallPos;


    private List<Ball> balls;
    private List<Block> blocks;
    private List<GreenOrb> greenOrbs;
    private List<Movable> moves;
    private List<GreenBall> greenBalls;
    public List<GreenBall> GreenBalls => greenBalls;


    private void Awake()
    {
        PoolManager.Init();
        ScoreManager.Init();


        instance = this;
        round = 1;

        totalBallPos = new Vector3(0f, -57f, 0f);
        balls = new List<Ball>();
        balls.Add(startBall);

        blocks = new List<Block>();
        greenOrbs = new List<GreenOrb>();
        moves = new List<Movable>();
        greenBalls = new List<GreenBall>();
    }

    private void OnEnable()
    {
        ScoreManager.OnBestScoreChange += OnBestScoreChange;
        ScoreManager.OnCurScoreChange += OnCurScoreChange;
    }

    private void OnDisable()
    {
        ScoreManager.OnBestScoreChange -= OnBestScoreChange;
        ScoreManager.OnCurScoreChange -= OnCurScoreChange;
    }

    private void Start()
    {
        OnBestScoreChange(ScoreManager.BestScore);


        inputController.OnBeginDragAction += _Position =>
        {
            firstClickPos = Camera.main.ScreenToWorldPoint(_Position) + Vector3.forward * 10f;
            shootDirectionLength = 0f;

            ballPreviewTransform.gameObject.SetActive(true);
            arrowTransform.gameObject.SetActive(true);
        };

        inputController.OnDragAction += _Position =>
        {
            secondClickPos = Camera.main.ScreenToWorldPoint(_Position) + Vector3.forward * 10f;

            shootDirection = secondClickPos - firstClickPos;
            shootDirectionLength = shootDirection.magnitude;

            shootDirection.Normalize();
            shootDirection = new Vector3(shootDirection.y >= 0 ? shootDirection.x : (shootDirection.x >= 0 ? 1 : -1), Mathf.Clamp(shootDirection.y, 0.2f, 1), 0f);


            arrowTransform.position = totalBallPos;
            arrowTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg);
            ballPreviewTransform.position = Physics2D.CircleCast(new Vector2(Mathf.Clamp(totalBallPos.x, -54, 54), Constants.BottomY), 1.7f, shootDirection, Mathf.Infinity, 1 << LayerMask.NameToLayer(Layers.WallLayerName) | 1 << LayerMask.NameToLayer(Layers.BlockLayerName)).centroid;

            RaycastHit2D hit = Physics2D.Raycast(totalBallPos, shootDirection, Mathf.Infinity, 1 << LayerMask.NameToLayer(Layers.WallLayerName) | 1 << LayerMask.NameToLayer(Layers.BlockLayerName));

            ballShootLineRenderer.SetPosition(0, totalBallPos);
            ballShootLineRenderer.SetPosition(1, ballPreviewTransform.position - shootDirection * 1.3f);
        };

        inputController.OnEndDragAction += () =>
        {
            ballShootLineRenderer.SetPosition(0, Vector3.zero);
            ballShootLineRenderer.SetPosition(1, Vector3.zero);


            totalBallPos = Vector3.zero;
            firstClickPos = Vector3.zero;


            ballPreviewTransform.gameObject.SetActive(false);
            arrowTransform.gameObject.SetActive(false);

            if(shootDirectionLength >= 1f)
            {
                //쏨
                StartCoroutine(ShootBalls());

                inputController.SetInputActive(false);
            }
        };

        inputController.SetInputActive(true);

        CreateBlocks();
    }

    public void OnGameOver()
    {
        inputController.SetInputActive(false);
        foreach (var block in blocks)
        {
            if (block.gameObject.activeSelf)
            {
                block.Suicide();
            }
        }

        foreach (var greenOrb in greenOrbs)
        {
            if(greenOrb.gameObject.activeSelf)
            {
                greenOrb.Suicide();
            }
        }
    }

    private void OnBestScoreChange(int _Score)
    {
        bestScoreText.text = "최고 기록 : " + _Score;
    }
    private void OnCurScoreChange(int _Score)
    {
        curScoreText.text = "현재 기록 : " + _Score;
    }

    public void SetTotalBallPos(Vector3 _Pos)
    {
        if (totalBallPos == Vector3.zero)
        {
            totalBallPos = _Pos;
        }
    }

    public void AddBall()
    {
        var ball = PoolManager.GetBall();
        ball.transform.position = totalBallPos;
        ball.gameObject.SetActive(true);
        balls.Add(ball);
    }

    private void CreateBlocks()
    {
        int count;
        int randBlock = Random.Range(0, 24);
        if (round <= 10)
        {
            count = randBlock < 16 ? 1 : 2;
        }
        else if (round <= 20)
        {
            count = randBlock < 8 ? 1 : (randBlock < 6 ? 2 : 3);
        }
        else if (round <= 40)
        {
            count = randBlock < 9 ? 2 : (randBlock < 18 ? 3 : 4);
        }
        else
        {
            count = randBlock < 8 ? 2 : randBlock < 16 ? 3 : (randBlock < 20 ? 4 : 5);
        }

        List<Vector3> spawnPoses = new List<Vector3>();
        for (int i = 0; i < spawnTransforms.Length; i++)
        {
            spawnPoses.Add(spawnTransforms[i].position);
        }

        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, spawnPoses.Count);

            var block = PoolManager.GetBlock();
            block.transform.position = spawnPoses[rand];
            block.SetNumber(round);
            block.gameObject.SetActive(true);
            if (!blocks.Contains(block))
            {
                blocks.Add(block);
                moves.Add(block);
            }

            spawnPoses.RemoveAt(rand);
        }

        var greenOrb = PoolManager.GetGreenOrb();
        greenOrb.SetActive(true);
        greenOrb.transform.position = spawnPoses[Random.Range(0, spawnPoses.Count)];
        var greenOrbComponent = greenOrb.GetComponent<GreenOrb>();
        if(!greenOrbs.Contains(greenOrbComponent))
        {
            greenOrbs.Add(greenOrbComponent);
        }
        if (!moves.Contains(greenOrbComponent))
        {
            moves.Add(greenOrbComponent);
        }

        //블럭들 내려줌
        foreach (var movableObj in moves)
        {
            if(movableObj.gameObject.activeSelf)
            {
                movableObj.MoveToBottom();
            }
        }

        foreach (var greenball in greenBalls)
        {
            greenball.MoveToTargetAndActiveOff(totalBallPos, AddBall);
        }
        greenBalls.Clear();

        //다 내려왔는지 체크함
        StartCoroutine(CheckAllMovableObjMovedone());
    }

    IEnumerator CheckAllMovableObjMovedone()
    {
        Func<bool> check = () =>
        {
            for (int i = 0; i < moves.Count; i++)
            {
                if(!moves[i].gameObject.activeSelf)
                {
                    continue;
                }

                if(moves[i].IsMoving)
                {
                    return false;
                }
            }
            return true;
        };

        yield return new WaitUntil(check);

        inputController.SetInputActive(true);
    }

    private IEnumerator ShootBalls()
    {
        //쏨
        foreach (var ball in balls)
        {
            ball.Shoot(shootDirection);
            yield return YieldContainer.GetWaitSeconds(Constants.BallShootDelayTime);
        }
        StartCoroutine(CheckAllBallsMoving());
    }

    private IEnumerator CheckAllBallsMoving()
    {
        Func<bool> checkBalls = () =>
        {
            foreach (var ball in balls)
            {
                if (ball.IsMoving)
                {
                    return false;
                }
            }
            return true;
        };
        yield return new WaitUntil(checkBalls);

        ++round;
        //다 끝나고 여기 부분
        CreateBlocks();
    }
}
