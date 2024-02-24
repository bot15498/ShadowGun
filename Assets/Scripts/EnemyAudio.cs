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
            StartCoroutine(OneShot("Alert", clip, enemy));
        };
        EnemyHP.onDeath += (GameObject enemy) => StartCoroutine(OneShot("Death", death, enemy));
    }

    IEnumerator OneShot(string name, AudioClip clip, GameObject enemy) {
        GameObject emitter = new() { name = name };
        emitter.transform.position = enemy.transform.position;
        emitter.transform.parent = enemy.transform;
        AudioSource src = emitter.AddComponent<AudioSource>();

        src.PlayOneShot(clip);
        while (src.isPlaying) {
            yield return new WaitForSeconds(.1f);
        }
        Destroy(emitter);
    }
}
