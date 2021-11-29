using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotator : MonoBehaviour
{
    [SerializeField] Vector3 _rotation = new Vector3(0,0,0.5f);
    [SerializeField] float _speedFactor = 1;
    private const float MAX_FPS = 60;

    // Update is called once per frame
    void Update()
    {
        if(isActiveAndEnabled)
            transform.Rotate(_rotation*Time.deltaTime * MAX_FPS * _speedFactor);
    }
}
