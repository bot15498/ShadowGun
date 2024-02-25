using System.Collections;
using UnityEngine;

using LibAudio;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField]
    private AudioClip gunfire;

    private void Awake() {
        Gun.onPlayerFire += () =>
            StartCoroutine(
                OneShot("Gunfire", gunfire, new(-9f), new(-1f), new(1f)));
    }

    IEnumerator OneShot(
        string name,
        AudioClip clip,
        DB vol,
        Semitone pitchMin,
        Semitone pitchMax
    ) {
        GameObject emitter = new() { name = name };
        emitter.transform.position = gameObject.transform.position;
        emitter.transform.parent = gameObject.transform;
        AudioSource src = emitter.AddComponent<AudioSource>();
        src.playOnAwake = false;

        src.volume = vol.Percent;
        src.pitch = Random.Range(pitchMin.Percent, pitchMax.Percent);

        src.PlayOneShot(clip);
        while (src.isPlaying) {
            yield return new WaitForSeconds(.1f);
        }
        Destroy(emitter);
    }
}
