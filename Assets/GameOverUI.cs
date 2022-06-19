using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MJ.Manager;
using UnityEngine.UI;

public sealed class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image panelImage;


    IEnumerator OnTriggerGameOverRoutine()
    {
        yield return YieldContainer.GetWaitSeconds(1f);
        //블럭들, 그린볼들, 구슬들 없앰


    }
}
