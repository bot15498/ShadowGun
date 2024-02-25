using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ExitLevelCondition
{
    KillAllEnemies,
    Survive
}

public class DeathManager : MonoBehaviour
{
    public ExitLevelCondition exitLevelCondition;
    public GameObject wintext;
    public TextMeshProUGUI timerText;
    SceneLoader sl;

    // kill everyone
    [SerializeField]
    private List<GameObject> enemies;

    // survive
    [SerializeField]
    private float timeToLiveSec = 30f;
    [SerializeField]
    private float currentLiveTime = 0f;

    void Start()
    {
        sl = GetComponent<SceneLoader>();
        switch (exitLevelCondition)
        {
            case ExitLevelCondition.KillAllEnemies:
                // Wait for all enemies to die
                break;
            case ExitLevelCondition.Survive:
                // live for certain time
                currentLiveTime = timeToLiveSec;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(exitLevelCondition)
        {
            case ExitLevelCondition.KillAllEnemies:
                // Wait for all enemies to die
                if(enemies.All(x => x == null))
                {
                    YouWin();
                }
                break;
            case ExitLevelCondition.Survive:
                // live for certain time
                currentLiveTime -= Time.deltaTime;
                if(currentLiveTime > 0)
                {
                    timerText.text = currentLiveTime.ToString("00.000");
                }
                else
                {
                    timerText.text = "00.000";
                }
                if (currentLiveTime <= 0)
                {
                    YouWin();
                }
                break;
        }
    }

    private void YouWin()
    {
        wintext.SetActive(true);
        StartCoroutine(Example2());
    }

    IEnumerator Example2()
    {

        yield return new WaitForSeconds(1.5f);
        sl.loadScene();

    }
}
