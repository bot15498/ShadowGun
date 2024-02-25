using System;
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
    [SerializeField] private AudioClip slide;

    // Used to check player input to control sound behavior
    private PlayerInput inputManager;
    private PlayerController playerController;

    // Unique audio channels
    private AudioSource stepPlayer;
    private AudioSource reloadPlayer;
    private AudioSource slidePlayer;

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
        playerController = transform.parent.GetComponent<PlayerController>();

        stepPlayer = gameObject.AddComponent<AudioSource>();
        stepPlayer.playOnAwake = false;
        DB stepVol = new(-6f);
        stepPlayer.volume = stepVol.Percent;

        reloadPlayer = gameObject.AddComponent<AudioSource>();
        reloadPlayer.playOnAwake = false;
        DB reloadVol = new(-13f);
        reloadPlayer.volume = reloadVol.Percent;
        reloadPlayer.clip = reload;

        slidePlayer = gameObject.AddComponent<AudioSource>();
        slidePlayer.playOnAwake = false;
        slidePlayer.loop = true;
        DB slideVol = new(-7f);
        slidePlayer.volume = slideVol.Percent;
        slidePlayer.clip = slide;

        // REGISTER EVENTS
        Gun.onPlayerFire += PlayGunfire;
        Gun.onPlayerReload += PlayReload;
        PlayerHP.onHit += PlayHurt;
        PlayerHP.onDeath += PlayDeath;
    }

    private void OnDestroy() {
        // GARBAGE COLLECT EVENTS
        Gun.onPlayerFire -= PlayGunfire;
        Gun.onPlayerReload -= PlayReload;
        PlayerHP.onHit -= PlayHurt;
        PlayerHP.onDeath -= PlayDeath;
    }

    private void Update() {
        // Footsteps
        if ((inputManager?.input.magnitude > DEADZONE) &
        (playerController.status != Status.sliding)) {
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

        // Slide
        if (playerController.status == Status.sliding) {
            if (!slidePlayer.isPlaying)
                slidePlayer.Play();
        } else {
            slidePlayer.Stop();
        }

    }

    private void PlayStep() {
        Semitone pitchMin = new(-2f);
        Semitone pitchMax = new(2f);
        stepPlayer.pitch = UnityEngine.Random.Range(pitchMin.Percent, pitchMax.Percent);
        stepPlayer.clip = step[UnityEngine.Random.Range(0, step.Length)];
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
        src.pitch = UnityEngine.Random.Range(pitchMin.Percent, pitchMax.Percent);

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
        AudioClip clip = hurt[UnityEngine.Random.Range(0, hurt.Length)];
        StartCoroutine(
            OneShot("Hurt", clip, new(-10f), new(-2f), new(2f)));
    }

    private void PlayDeath(GameObject player) {
        StartCoroutine(
            OneShot("Die", die, new(-10f), new(0f), new(0f)));
    }
}
