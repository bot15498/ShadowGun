using UnityEngine;

public class AudioOneshot : AudioChannel {
    new protected void Play() {
        if (src.isPlaying) return;
        src.clip = clip;
        src.Play();
    }
}
