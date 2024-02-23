using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiActiveToggle : MonoBehaviour
{
    public GameObject objectToToggle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
