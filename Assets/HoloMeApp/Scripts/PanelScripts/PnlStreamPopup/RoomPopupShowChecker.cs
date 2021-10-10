using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class RoomPopupShowChecker : MonoBehaviour
{

    public bool CanShow() {
        return true;
    }

    // добавить при каких условиях можетоткрывтаься.
    // подписаться на все события при которых будет окно закрываться (так как некоторые окна могу открываться с задержкой и нужно прервать отображения popup) 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
