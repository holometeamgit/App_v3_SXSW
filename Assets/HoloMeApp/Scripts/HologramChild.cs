using UnityEngine;

public abstract class HologramChild : MonoBehaviour
{
    public abstract void SetParent(Transform parent);
    public abstract void UpdateOffset(Vector3 position);
}
