using UnityEngine;

public class LiveStreamQuadController : HologramChild
{
    public override void SetParent(Transform parent)
    {
        transform.SetParent(parent, false);
        transform.localPosition = Vector3.zero;
        transform.Rotate(new Vector3(0, 0, 0));
    }

    public override void UpdateOffset(Vector3 position)
    {
    }
}
