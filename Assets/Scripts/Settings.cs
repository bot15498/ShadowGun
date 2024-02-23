using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Instance;

    public static float sensitivitySettings;

    private void Awake()
    {
        sensitivitySettings = 0.6f;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Debug.Log(sensitivitySettings);
    }

    public static void changeSensitivity(float sensitivityInput)
    {
        sensitivitySettings = sensitivityInput;
    }


}
