using System.Collections;
using System.Collections.Generic;
using UMI;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    void Awake() {
        MobileInput.Init();
    }
}
