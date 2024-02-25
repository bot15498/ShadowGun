using System.Collections;
using UnityEngine;

using LibAudio;

public class EnemyAudio : MonoBehaviour {
    [SerializeField]
    private AudioClip[] alert;
    [SerializeField]
    private AudioClip[] death;
    [SerializeField]
    private AudioClip[] shatter;

    private void Start() {
        EnemyAiBase.onAlert += PlayAlert;
        EnemyHP.onDeath += PlayDeath;
        DestroyShadow.onShatter += PlayShatter;
    }

    private void OnDestroy() {
        EnemyAiBase.onAlert -= PlayAlert;
        EnemyHP.onDeath -= PlayDeath;
        DestroyShadow.onShatter -= PlayShatter;
    }

    IEnumerator OneShot(
        string name,
        AudioClip clip,
        GameObject enemy,
        DB vol,
        Semitone pitchMin,
        Semitone pitchMax
    ) {
        GameObject emitter = new() { name = name };
        emitter.transform.position = enemy.transform.position;
        AudioSource src = emitter.AddComponent<AudioSource>();
        src.playOnAwake = false;

        src.spatialBlend = 0.9f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.minDistance = 1.0f;
        src.maxDistance = 30.0f;

        src.volume = vol.Percent;
        src.pitch = Random.Range(pitchMin.Percent, pitchMax.Percent);

        src.PlayOneShot(clip);
        while (src.isPlaying) {
            yield return new WaitForSeconds(.1f);
        }
        Destroy(emitter);
    }

    private void PlayAlert(GameObject enemy) {
        AudioClip clip = alert[Random.Range(0, alert.Length)];
        StartCoroutine(
            OneShot("Alert", clip, enemy, new(0f), new(-4f), new(4f)));
    }
    private void PlayDeath(GameObject enemy) {
        AudioClip clip = death[Random.Range(0, death.Length)];
        StartCoroutine(
            OneShot("Death", clip, enemy, new(-6f), new(-4f), new(4f)));
    }
    private void PlayShatter(GameObject shadow) {
        AudioClip clip = shatter[Random.Range(0, shatter.Length)];
        StartCoroutine(
            OneShot("Break", clip, shadow, new(-6f), new(-4f), new(4f)));
    }
}
