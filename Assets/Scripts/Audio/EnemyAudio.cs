using System.Collections;
using UnityEngine;

public class EnemyAudio : MonoBehaviour {
    [SerializeField]
    private AudioClip[] alert;
    [SerializeField]
    private AudioClip[] death;
    [SerializeField]
    private AudioClip[] shatter;

    private void Awake() {
        EnemyAiBase.onAlert += (GameObject enemy) => {
            AudioClip clip = alert[Random.Range(0, alert.Length - 1)];
            StartCoroutine(OneShot("Alert", clip, enemy, 1.0f, 0.5f, 2.0f));
        };
        EnemyHP.onDeath += (GameObject enemy) => {
            AudioClip clip = death[Random.Range(0, alert.Length - 1)];
            StartCoroutine(OneShot("Death", clip, enemy, 0.7f, 0.5f, 2.0f));
        };
        DestroyShadow.onShatter += (GameObject shadow) => {
            AudioClip clip = shatter[Random.Range(0, shatter.Length - 1)];
            StartCoroutine(OneShot("Break", clip, shadow, 0.7f, 0.5f, 2.0f));
        };
    }

    IEnumerator OneShot(
        string name,
        AudioClip clip,
        GameObject enemy,
        float vol = 1.0f,
        float pitchMin = 1.0f,
        float pitchMax = 1.0f
    ) {
        GameObject emitter = new() { name = name };
        emitter.transform.position = enemy.transform.position;
        AudioSource src = emitter.AddComponent<AudioSource>();
        src.playOnAwake = false;

        src.spatialBlend = 0.9f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.minDistance = 1.0f;
        src.maxDistance = 30.0f;

        src.volume = vol;
        if (pitchMin > 0)
            src.pitch = Random.Range(pitchMin, pitchMax);

        src.PlayOneShot(clip);
        while (src.isPlaying) {
            yield return new WaitForSeconds(.1f);
        }
        Destroy(emitter);
    }
}
