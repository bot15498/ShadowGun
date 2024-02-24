using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour

{
    public Animator anim;
    public int SceneToLoad;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadScene()
    {
        anim.Play("FadeOut");
        StartCoroutine(Example());
        
    }

    IEnumerator Example()
    {
       
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneToLoad);

    }
}
