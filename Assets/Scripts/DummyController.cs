///////////////////////////////////////////////////
// Move cube back and forth to test ShadowObject //
///////////////////////////////////////////////////

using UnityEngine;

public class DummyController : MonoBehaviour
{
    [SerializeField] private float range = 4.0f;
    private Transform dummy;
    private Vector3 start;

    private bool isReversing = false;
    private float currOffset = 0.0f;

    private void Start() {
        Init();
    }

    void Update() {
        UpdatePos();
    }

    private void Init() {
        dummy = gameObject.GetComponent<Transform>();
        start = dummy.position;
    }

    private void UpdatePos() {
        float dt = Time.deltaTime;
        if (isReversing)
            currOffset -= dt;
        else currOffset += dt;

        if (currOffset > range) {
            currOffset = range;
            isReversing = true;
        } else if (currOffset < -range) {
            currOffset = -range;
            isReversing = false;
        }

        Vector3 newPos = start;
        newPos.x += currOffset;
        dummy.position = newPos;
    }
}
