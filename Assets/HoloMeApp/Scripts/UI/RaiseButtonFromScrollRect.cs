using UnityEngine;

public class RaiseButtonFromScrollRect : MonoBehaviour {
    //[SerializeField]
    //private RectTransform contentContainer;

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

    [SerializeField]
    private Vector2 startPosition;

    [SerializeField]
    private Vector2[] positions;

    //private bool limitReached;



    private void OnEnable() {
        //transform.GetComponent<RectTransform>().position = defaultPosition.position;
        //lastChildCount = 0;
        //limitReached = false;
        transform.GetComponent<RectTransform>().anchoredPosition = startPosition;
        moveIndex = 0;
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
            transform.GetComponent<RectTransform>().anchoredPosition = positions[moveIndex];
            moveIndex++;
        }
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
