using UnityEngine;

public class PlayerFireSFX : AudioChannel {
    private void Start() {
        Gun.onPlayerFire += Play;
    }
}
