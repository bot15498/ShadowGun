using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ExitLevelCondition
{
    KillAllEnemies,
    Survive
}

public class DeathManager : MonoBehaviour
{
    public ExitLevelCondition exitLevelCondition;

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
                currentLiveTime += Time.deltaTime;
                if (currentLiveTime >= timeToLiveSec)
                {
                    YouWin();
                }
                break;
        }
    }

    private void YouWin()
    {

    }
}
