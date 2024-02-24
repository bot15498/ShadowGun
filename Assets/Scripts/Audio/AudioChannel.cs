using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioChannel : MonoBehaviour {
    protected AudioSource src;
    [SerializeField] AudioClip clip;
    private void Awake() {
        src = GetComponent<AudioSource>();
        src.mute = false;
        src.playOnAwake = false;
        src.loop = false;
    }
    protected void Play() {
        src.clip = clip;
        src.Play();
    }
}
