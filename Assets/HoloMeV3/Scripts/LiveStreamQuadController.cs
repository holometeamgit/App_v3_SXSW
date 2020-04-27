using UnityEngine;

public class LiveStreamQuadController : HologramChild
{
    public override void SetParent(Transform parent)
    {
        transform.SetParent(parent, false);
        transform.localPosition = Vector3.zero;
    }

    public override void UpdateOffset(Vector3 position)
    {
        //transform.localPosition = position;
    }
}
