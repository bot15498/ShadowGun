using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ObjectiveText : MonoBehaviour
{
    public GameObject objtextobject;
    public TextMeshProUGUI objtext;

    public bool destroyallshadows;

    // Start is called before the first frame update
    void Start()
    {
        objtext = objtextobject.GetComponent<TextMeshProUGUI>();
        

        if(destroyallshadows == true)
        {
            StartCoroutine(Example());
            
           
        }
    }

    IEnumerator Example()
    {

        yield return new WaitForSeconds(2f);
        objtextobject.SetActive(true);
        objtext.text = "Destroy";
        yield return new WaitForSeconds(1f);
        objtext.text = "All";
        yield return new WaitForSeconds(1f);
        objtext.text = "Shadows";
        yield return new WaitForSeconds(1f);
        objtextobject.SetActive(false);

    }



}
