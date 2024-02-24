using System.Collections;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField]
    private AudioClip gunfire;

    private void Awake() {
        Gun.onPlayerFire += () =>
            StartCoroutine(OneShot("Gunfire", gunfire, 0.95f, 1.05f));
    }

    IEnumerator OneShot(
        string name,
        AudioClip clip,
        float pitchMin = 1.0f,
        float pitchMax = 1.0f
    ) {
        GameObject emitter = new() { name = name };
        emitter.transform.position = gameObject.transform.position;
        emitter.transform.parent = gameObject.transform;
        AudioSource src = emitter.AddComponent<AudioSource>();
        src.playOnAwake = false;

        if (pitchMin > 0)
            src.pitch = Random.Range(pitchMin, pitchMax);

        src.PlayOneShot(clip);
        while (src.isPlaying) {
            yield return new WaitForSeconds(.1f);
        }
        Destroy(emitter);
    }
}
