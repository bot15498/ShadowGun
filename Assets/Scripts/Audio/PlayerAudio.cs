using System.Collections;
using UnityEngine;

using LibAudio;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip gunfire;
    [SerializeField] private AudioClip[] step;

    private PlayerInput inputManager;
    private AudioSource stepPlayer;

    // footstep behavior
    private float ticksMoved = 0f;

    // CONSTANTS TO CONFIG FOOTSTEP BEHAVIOR
    private readonly float SECS_PER_STEP = 0.4f;
    private readonly float SECS_PER_RUN = 0.2f;
    private readonly float START_DEADZONE = 0.1f;
    private readonly float STOP_DEADZONE = 0.5f;

    private void Start() {
        inputManager = transform.parent.GetComponent<PlayerInput>();
        stepPlayer = gameObject.AddComponent<AudioSource>();
        stepPlayer.playOnAwake = false;

        DB vol = new(-6f);
        stepPlayer.volume = vol.Percent;

        Gun.onPlayerFire += PlayGunfire;
    }

    private void OnDestroy() {
        Gun.onPlayerFire -= PlayGunfire;
    }

    private void Update() {
        Debug.Log(inputManager.input.magnitude);

        // Footsteps
        if (inputManager?.input.magnitude < STOP_DEADZONE) {
            stepPlayer.Stop();
        }
        else if (inputManager?.input.magnitude > START_DEADZONE) {
            ticksMoved += Time.deltaTime;

            if (inputManager.run) {
                if (ticksMoved >= SECS_PER_RUN)
                    PlayStep();
            } else {
                if (ticksMoved >= SECS_PER_STEP)
                    PlayStep();
            }
        }
    }

    private void PlayStep() {
        Semitone pitchMin = new(-2f);
        Semitone pitchMax = new(2f);
        stepPlayer.pitch = Random.Range(pitchMin.Percent, pitchMax.Percent);
        stepPlayer.clip = step[Random.Range(0, step.Length)];
        stepPlayer.Play();
        ticksMoved = 0f;
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
}
