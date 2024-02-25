using UnityEngine;

using LibAudio;

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
        DB vol = new(-14f);
        musicPlayer.volume = vol.Percent;
        musicPlayer.clip = music;
        musicPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
