using System.Collections;
using UnityEngine;

public class EnemyAudio : MonoBehaviour {
    [SerializeField]
    private AudioClip[] alertSounds;

    private void Awake() {
        EnemyAiBase.onAlert += (GameObject enemy) => {
            AudioClip clip = alertSounds[Random.Range(0, alertSounds.Length - 1)];
            StartCoroutine(OneShot("Alert", clip, enemy));
        };
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
