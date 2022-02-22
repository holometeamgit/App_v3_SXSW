using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class fires events when swipe is detected
/// </summary>
public class SwipeGestureEvents : MonoBehaviour {
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public bool detectSwipeOnlyAfterRelease = false;

    public float SWIPE_THRESHOLD = 20f;

    [SerializeField]
    UnityEvent OnSwipeRighte;
    [SerializeField]
    UnityEvent OnSwipeLefte;
    [SerializeField]
    UnityEvent OnSwipeUpe;
    [SerializeField]
    UnityEvent OnSwipeDowne;

    private void Update() {

        foreach (Touch touch in Input.touches) {
            if (touch.phase == TouchPhase.Began) {
                fingerUp = touch.position;
                fingerDown = touch.position;
            }

            //Detects Swipe while finger is still moving
            if (touch.phase == TouchPhase.Moved) {
                if (!detectSwipeOnlyAfterRelease) {
                    fingerDown = touch.position;
                    CheckSwipe();
                }
            }

            //Detects swipe after finger is released
            if (touch.phase == TouchPhase.Ended) {
                fingerDown = touch.position;
                CheckSwipe();
            }
        }
    }

    private void CheckSwipe() {
        //Check if Vertical swipe
        if (VerticalMove() > SWIPE_THRESHOLD && VerticalMove() > HorizontalValMove()) {

            if (fingerDown.y - fingerUp.y > 0)//up swipe
            {
                OnSwipeUp();
            } else if (fingerDown.y - fingerUp.y < 0)//Down swipe
              {
                OnSwipeDown();
            }
            fingerUp = fingerDown;
        }

        //Check if Horizontal swipe
        else if (HorizontalValMove() > SWIPE_THRESHOLD && HorizontalValMove() > VerticalMove()) {

            if (fingerDown.x - fingerUp.x > 0)//Right swipe
            {
                OnSwipeRight();
            } else if (fingerDown.x - fingerUp.x < 0)//Left swipe
              {
                OnSwipeLeft();
            }
            fingerUp = fingerDown;
        }
    }

    private float VerticalMove() {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    private float HorizontalValMove() {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    private void OnSwipeUp() {
        Debug.Log("Swipe UP");
        OnSwipeUpe?.Invoke();
    }

    private void OnSwipeDown() {
        Debug.Log("Swipe Down");
        OnSwipeDowne?.Invoke();
    }

    private void OnSwipeLeft() {
        Debug.Log("Swipe Left");
        OnSwipeLefte?.Invoke();
    }

    private void OnSwipeRight() {
        Debug.Log("Swipe Right");
        OnSwipeRighte?.Invoke();
    }
}