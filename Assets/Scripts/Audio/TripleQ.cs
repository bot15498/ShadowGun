using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleQ : MonoBehaviour
{
    private AudioSource musicPlayer;

    [SerializeField] private AudioClip music;

    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = gameObject.AddComponent<AudioSource>();
        musicPlayer.loop = true;
        musicPlayer.playOnAwake = true;
        musicPlayer.clip = music;
        musicPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
