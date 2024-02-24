using System.Collections;
using UnityEngine;

public class EnemyAudio : MonoBehaviour {
    [SerializeField]
    private AudioClip[] alert;
    [SerializeField]
    private AudioClip death;

    private void Awake() {
        EnemyAiBase.onAlert += (GameObject enemy) => {
            AudioClip clip = alert[Random.Range(0, alert.Length - 1)];
            StartCoroutine(OneShot("Alert", clip, enemy, 0.5f, 2.0f));
        };
        EnemyHP.onDeath += (GameObject enemy) =>
            StartCoroutine(OneShot("Death", death, enemy, 0.5f, 2.0f));
    }

    IEnumerator OneShot(
        string name,
        AudioClip clip,
        GameObject enemy,
        float pitchMin = 1.0f,
        float pitchMax = 1.0f
    ) {
        GameObject emitter = new() { name = name };
        emitter.transform.position = enemy.transform.position;
        AudioSource src = emitter.AddComponent<AudioSource>();

        if (pitchMin > 0)
            src.pitch = Random.Range(pitchMin, pitchMax);

        src.PlayOneShot(clip);
        while (src.isPlaying) {
            yield return new WaitForSeconds(.1f);
        }
        Destroy(emitter);
    }
}
