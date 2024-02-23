using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioFPS : MonoBehaviour {
    AudioSource audioSource;
    [SerializeField] AudioClip playerFireSFX;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = false;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        Gun.onPlayerFire += PlayerFireSFX;
    }

    private void PlayerFireSFX() {
        audioSource.clip = playerFireSFX;
        audioSource.Play();
    }
}
