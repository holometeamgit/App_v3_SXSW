using System.Collections;
using UnityEngine;

public class AutoDisableTimer : MonoBehaviour
{
    [SerializeField]
    float waitTime = 3;

    private void OnEnable()
    {
        StartCoroutine(DisableRoutine());
    }
    
    IEnumerator DisableRoutine()
    {
        yield return new WaitForSeconds(waitTime);
        transform.gameObject.SetActive(false);
    }

}
