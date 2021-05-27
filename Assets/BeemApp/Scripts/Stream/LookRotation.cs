using UnityEngine;


public class LookRotation : MonoBehaviour {
    [SerializeField]
    private Transform lookTarget;
    private float damping = 15.0f;
    private float lerpFactorCallibration = 0.5f;
    private bool hasAddRotation = true;
    private bool disableXRotation;

    private void Awake() {
        lookTarget = FindObjectOfType<Camera>().transform;
    }

    protected void Update() {
        if (lookTarget == null)
            return;

        var lookPos = lookTarget.position - transform.position;
        lookPos.y = 0;
        var rotation = (lookPos != Vector3.zero) ? Quaternion.LookRotation(lookPos) : Quaternion.identity;

        if (disableXRotation) {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        } else {
            if (hasAddRotation) {
                var addRotation = Quaternion.Euler(new Vector3(-lookTarget.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z));
                rotation = Quaternion.Lerp(rotation, addRotation, lerpFactorCallibration);
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w), Time.deltaTime * damping);
        }
    }
}

