using UnityEngine;
using TMPro;
using MJ.Manager;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public sealed class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject titleTextObj;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image panelImage;
    [SerializeField] private Button restartButton;
    private bool isOnTriggerGameOver = false;

    public void OnClickRestart()
    {
        var curScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(curScene.name);
    }

    public void OnTriggerGameOver()
    {
        if(isOnTriggerGameOver)
        {
            return;
        }

        isOnTriggerGameOver = true;
        StartCoroutine(OnTriggerGameOverRoutine());
    }

    IEnumerator OnTriggerGameOverRoutine()
    {
        yield return YieldContainer.GetWaitSeconds(1f);
        //블럭들, 그린볼들, 구슬들 없앰
        GameManager.Instance.OnGameOver();

        panelImage.gameObject.SetActive(true);

        while (panelImage.color.a < 1f)
        {
            var color = panelImage.color;
            color.a += Time.deltaTime * 3f;
            panelImage.color = color;
            yield return null;
        }

        titleTextObj.SetActive(true);

        yield return YieldContainer.GetWaitSeconds(1f);
        scoreText.text = "점수 : " + ScoreManager.CurScore;
        scoreText.gameObject.SetActive(true);

        yield return YieldContainer.GetWaitSeconds(1f);
        restartButton.gameObject.SetActive(true);
    }
}
