using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SignalBusMonoBehaviour : MonoBehaviour
{
    SignalBus _signalBus;

    public SignalBus SignalBus {
        get { return _signalBus; }
    }

    [Inject]
    public void Construct(SignalBus signalBus) {
        _signalBus = signalBus;
    }
}
