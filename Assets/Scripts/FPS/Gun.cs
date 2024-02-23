using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Gun : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform gunBarrel;
    public GameObject bullet;
    public int ammocountMax;
    public int ammocount;
    public float firerate;
    float fireratetimer;
    public float reloadtimer;
    public float timer;
    bool isreloading;
    bool canfire;
    public TextMeshProUGUI ammotext;
    public Animator anim;
    public GameObject reloadText;


    void Start()
    {
        ammocount = ammocountMax;
        timer = 0;
        fireratetimer = 0;
        isreloading = false;
        canfire = true;
        reloadText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if(ammocount <= 5 && isreloading == false)
        {
            reloadText.SetActive(true);
        }
        

        ammotext.text = ammocount.ToString() + "/" + ammocountMax.ToString();
        if (Input.GetKey(KeyCode.Mouse0) && ammocount > 0 && isreloading == false && canfire == true)
        {
            Instantiate(bullet, gunBarrel.position, gunBarrel.rotation);
            ammocount -= 1;
            
            
            canfire = false;

        }
        if(Input.GetKey(KeyCode.Mouse0) && ammocount >0 && isreloading == false)
        {
            anim.Play("Fire");
        }
        else if(isreloading == false)
        {
            anim.Play("Idle");
        }


        if(ammocount == 0 || (Input.GetKeyDown(KeyCode.R)&& ammocount != ammocountMax))
        {

            anim.Play("reload");
            isreloading = true;
            reloadText.SetActive(false);

        }
        if(isreloading == true)
        {
            timer += Time.deltaTime;
        }


        if(timer>= reloadtimer)
        {
            isreloading = false;
            
            timer = 0;
            ammocount = ammocountMax;
        }
        if(canfire == false)
        {
            fireratetimer += Time.deltaTime;
            if(fireratetimer >= firerate)
            {
                canfire = true;
                fireratetimer = 0;
            }
        }

    }
}
