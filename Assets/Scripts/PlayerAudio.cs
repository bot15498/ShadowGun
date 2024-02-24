using System.Collections;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField]
    private AudioClip gunfire;

    private void Awake() {
        Gun.onPlayerFire += () => StartCoroutine(OneShot("Gunfire", gunfire));
    }

    IEnumerator OneShot(string name, AudioClip clip) {
        GameObject emitter = new() { name = name };
        emitter.transform.position = gameObject.transform.position;
        emitter.transform.parent = gameObject.transform;
        AudioSource src = emitter.AddComponent<AudioSource>();
        
        src.PlayOneShot(clip);
        while (src.isPlaying) {
            yield return new WaitForSeconds(.1f);
        }
        Destroy(emitter);
    }
}
