using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIThumbnail : MonoBehaviour {

    public virtual void SetPlayAction(Action<StreamJsonData.Data> OnPlayClick) { }
    public virtual void SetTeaserPlayAction(Action<StreamJsonData.Data> OnTeaserClick) { }
    public virtual void SetBuyAction(Action<StreamJsonData.Data> OnBuyClick) { }
    public virtual void SetShareAction(Action<StreamJsonData.Data> OnShareClick) { }

    public virtual void AddData(ThumbnailElement element) { }

    public virtual void ThumbnailClick() { }
    public virtual void Play() { }
    public virtual void PlayTeaser() { }
    public virtual void Buy() { }
    public virtual void Share() { }

    public virtual void LockToPress(bool isLook) { }

    public virtual void Deactivate() { }

    public virtual void Activate() { }

}
