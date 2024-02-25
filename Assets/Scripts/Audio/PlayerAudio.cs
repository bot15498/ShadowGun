using System.Collections;
using UnityEngine;

using LibAudio;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip gunfire;
    [SerializeField] private AudioClip[] step;
    [SerializeField] private AudioClip reload;
    [SerializeField] private AudioClip[] hurt;
    [SerializeField] private AudioClip die;

    private PlayerInput inputManager;
    private AudioSource stepPlayer;
    private AudioSource reloadPlayer;

    // footstep behavior
    private float secsToStep = 0.38f;

    // CONSTANTS TO CONFIG FOOTSTEP BEHAVIOR
    private readonly float SECS_PER_STEP = 0.4f;
    private readonly float SECS_PER_RUN = 0.2f;
    private readonly float DEADZONE = 0.5f;
    private readonly float HEADSTART = 0.38f;

    // reload behavior
    private float reloadTimer = 0f;
    private bool isreloading = false;

    // CONSTANTS TO CONFIG RELOAD BEHAVIOR
    private readonly float RELOAD_COOLDOWN = 2f;

    private void Start() {
        inputManager = transform.parent.GetComponent<PlayerInput>();

        stepPlayer = gameObject.AddComponent<AudioSource>();
        stepPlayer.playOnAwake = false;
        DB stepVol = new(-6f);
        stepPlayer.volume = stepVol.Percent;

        reloadPlayer = gameObject.AddComponent<AudioSource>();
        reloadPlayer.playOnAwake = false;
        DB reloadVol = new(-13f);
        reloadPlayer.volume = reloadVol.Percent;
        reloadPlayer.clip = reload;

        Gun.onPlayerFire += PlayGunfire;
        Gun.onPlayerReload += PlayReload;
        PlayerHP.onHit += PlayHurt;
        PlayerHP.onDeath += PlayDeath;
    }

    private void OnDestroy() {
        Gun.onPlayerFire -= PlayGunfire;
        Gun.onPlayerReload -= PlayReload;
        PlayerHP.onHit -= PlayHurt;
        PlayerHP.onDeath -= PlayDeath;
    }

    private void Update() {
        // Footsteps
        if (inputManager?.input.magnitude > DEADZONE) {
            secsToStep += Time.deltaTime;

            if (inputManager.run) {
                if (secsToStep >= SECS_PER_RUN)
                    PlayStep();
            } else {
                if (secsToStep >= SECS_PER_STEP)
                    PlayStep();
            }
        } else {
            stepPlayer.Stop();
            secsToStep = HEADSTART;
        }

        // Reload
        if (isreloading) {
            reloadTimer += Time.deltaTime;
            if (reloadTimer > RELOAD_COOLDOWN) {
                reloadTimer = 0f;
                isreloading = false;
            }
        }
    }

    private void PlayStep() {
        Semitone pitchMin = new(-2f);
        Semitone pitchMax = new(2f);
        stepPlayer.pitch = Random.Range(pitchMin.Percent, pitchMax.Percent);
        stepPlayer.clip = step[Random.Range(0, step.Length)];
        stepPlayer.Play();
        secsToStep = 0f;
    }
    private void PlayReload() {
        if (isreloading) return;
        reloadPlayer.Play();
        isreloading = true;
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

    private void PlayGunfire() {
        StartCoroutine(
            OneShot("Gunfire", gunfire, new(-9f), new(-1f), new(1f)));
    }

    private void PlayHurt(GameObject player) {
        AudioClip clip = hurt[Random.Range(0, hurt.Length)];
        StartCoroutine(
            OneShot("Hurt", clip, new(-10f), new(-2f), new(2f)));
    }

    private void PlayDeath(GameObject player) {
        StartCoroutine(
            OneShot("Die", die, new(-10f), new(0f), new(0f)));
    }
}
