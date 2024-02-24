using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiActiveToggle : MonoBehaviour
{
    public GameObject objectToToggle;
    bool isactive;
    public GameObject gameobjectToggle;
    
    // Start is called before the first frame update
    void Start()
    {
        isactive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameobjectToggle != null)
        {
            gameobjectToggle.SetActive(isactive);
        }
    }

    public void toggleactive()
    {
        isactive = !isactive;
    }



    public void setactive()
    {
        objectToToggle.SetActive(true);
    }

    public void deactivate()
    {
        objectToToggle.SetActive(false);
    }
}
