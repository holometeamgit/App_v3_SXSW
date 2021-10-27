using UnityEngine;

public class RaiseButtonFromScrollRect : MonoBehaviour {
    [SerializeField]
    private RectTransform contentContainer;

    //[SerializeField]
    //private float offsetValue;

    //[SerializeField]
    //private float finalPosition;

    //[Tooltip("How many times to raise until stopping")]
    //[SerializeField]
    //private int raiseLimit;
    //private int lastChildCount;

    //private RectTransform defaultPosition;

    private int moveIndex;

    //[SerializeField]
    //private Vector2[] positions;    
    
    [SerializeField]
    private RectTransform[] positions;

    //private bool limitReached;



    private void OnEnable() {
        //transform.GetComponent<RectTransform>().position = defaultPosition.position;
        //lastChildCount = 0;
        //limitReached = false;
        //transform.GetComponent<RectTransform>().anchoredPosition = startPosition;
        moveIndex = 0;
        UpdatePosition();
    }

    //public void UpdateOffset(int multiplier) {
    //    print("UPDATE OFFSET CALLED " + multiplier);
    //    if (multiplier >= raiseLimit) {
    //        if (!limitReached)
    //            transform.GetComponent<RectTransform>().position = new Vector3(defaultPosition.position.x, finalPosition, defaultPosition.position.z);
    //        limitReached = true;
    //        return;
    //    }
    //    var rectTransform = transform.GetComponent<RectTransform>();
    //    //rectTransform.position = rectTransform.position + new Vector3(0, offsetValue, 0);
    //    //rectTransform.position = defaultPosition.position + new Vector3(0, offsetValue* multiplier, 0);
    //    transform.GetComponent<RectTransform>().position += new Vector3(0, offsetValue, 0);
    //    //raiseCount++ ;
    //}

    public void MoveNext() {
        print("MOVE NEXT CALLED " + moveIndex);
        if (moveIndex < positions.Length) {
            transform.GetComponent<RectTransform>().anchorMin = positions[moveIndex].anchorMin;
            transform.GetComponent<RectTransform>().anchorMax = positions[moveIndex].anchorMax;
            transform.GetComponent<RectTransform>().sizeDelta = positions[moveIndex].sizeDelta;
            transform.GetComponent<RectTransform>().anchoredPosition = positions[moveIndex].anchoredPosition;
            moveIndex++;
        }
    }

    public void UpdatePosition() {

        int activeChatMessages = 0;

        for (int i = 0; i < contentContainer.childCount; i++) {
            if (contentContainer.GetChild(i).gameObject.activeInHierarchy) {
                activeChatMessages++;
            }
        }

        print("UpdatePosition" + activeChatMessages);
        transform.GetComponent<RectTransform>().sizeDelta = positions[activeChatMessages < positions.Length ? activeChatMessages : positions.Length - 1].sizeDelta;
        transform.GetComponent<RectTransform>().anchoredPosition = positions[activeChatMessages < positions.Length ? activeChatMessages : positions.Length - 1].anchoredPosition;
        transform.GetComponent<RectTransform>().anchorMin = positions[activeChatMessages < positions.Length ? activeChatMessages : positions.Length - 1].anchorMin;
        transform.GetComponent<RectTransform>().anchorMax = positions[activeChatMessages < positions.Length ? activeChatMessages : positions.Length - 1].anchorMax;
    }

    //private void Update() {
    //    if (lastChildCount != contentContainer.childCount) {
    //        UpdateOffset(contentContainer.childCount);
    //        lastChildCount = contentContainer.childCount;
    //    }
    //    //for (int i = 0; i < contentContainer.childCount; i++) {
    //    //}
    //}

}
