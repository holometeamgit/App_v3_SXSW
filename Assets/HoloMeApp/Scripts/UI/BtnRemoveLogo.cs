using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.SSO;

public class BtnRemoveLogo : MonoBehaviour {
    public void RemoveLogo() {
        CallBacks.onRemoveLogo?.Invoke();
    }
}
