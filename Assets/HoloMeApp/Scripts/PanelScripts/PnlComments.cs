using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.UI;
using Beem.Content;
using System;
using TMPro;
using UnityEngine.UI;

public class PnlComments : MonoBehaviour
{
	public delegate CommentJsonData GetItemByOrdinalID(int index);
	public event GetItemByOrdinalID onGetItemByOrdinalIndex;
	public Action onRefresh;
	public Action onLoadNext;
	public Action<int> onOpen;
	public Action onClose;
	public Action<string> onPost;

	[SerializeField]
    private InfiniteScroll Scroll;

//	[SerializeField]
	private UICommentElement uiCommentElementPrefab;

	[SerializeField]
	TMP_InputField _commentInputField;
	[SerializeField]
	Button _postBtn;

	private bool _isCanOpen;

	#region UI
	public void OpenComments(int contentId) {
		HelperFunctions.DevLog("pnlcomments OpenComments " + contentId);
		_isCanOpen = true;
		uiCommentElementPrefab = Scroll.Prefab.GetComponent<UICommentElement>();
		Scroll.IsPullBottom = true;
		onOpen?.Invoke(contentId);
	}

	public void CloseComments() {
		_isCanOpen = false;
		//animator set value close
	}

	public void Post() {
		if (string.IsNullOrWhiteSpace(_commentInputField.text))
			return;
		_postBtn.interactable = false;
		onPost?.Invoke(_commentInputField.text);
    }

	public void ShowComment() {
		//animator open comment 
	}

	public void HideComment() {
		//animator hide comment 
	}
	#endregion

	#region response from model
	//calls after posted to backend
	public void OnPost() {
		_commentInputField.text = "";
		_postBtn.interactable = true;
	}

	//calls after didn't post to backend
	public void OnFailPost() {
		_postBtn.interactable = true;
		//TODO show msg for user
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="count">count comments</param>
	/// <param name="pullCount">new loaded comments count</param>
	public void OnApplyDataTo(int count, int pullCount) {
		HelperFunctions.DevLog("count " + count + " pullCount " + pullCount);
		if (!_isCanOpen)
			return;

		if (!gameObject.activeSelf)
			OnOpen();

		Scroll.ApplyDataTo(count, pullCount, InfiniteScroll.Direction.Bottom);
	}

	public void OnAllDataLoaded() {
		Scroll.IsPullBottom = false;
	}

	#endregion

	#region response from ui

	void OnFillItem(int index, GameObject item) {
		HelperFunctions.DevLog("OnFillItem " + index);
		//if (onGetItemByOrdinalID == null)
		//	return;
		CommentJsonData commentData = onGetItemByOrdinalIndex(index);
		item.GetComponentInChildren<UICommentElement>().UpdateData(commentData.user, commentData.body, commentData.created_at, 0, commentData.id);
	}

	int OnHeightItem(int index) {
		HelperFunctions.DevLog("OnHeightItem " + index);
		CommentJsonData commentData = onGetItemByOrdinalIndex(index);

		return Mathf.CeilToInt(uiCommentElementPrefab.GetRequiredHeight(commentData.user, commentData.body));
	}

	void OnPullItem(InfiniteScroll.Direction direction) {
		HelperFunctions.DevLog("OnPullItem direction " + direction);
		if (direction == InfiniteScroll.Direction.Top) {
			onRefresh?.Invoke();
			Scroll.IsPullBottom = true;
		} else {
			onLoadNext?.Invoke();
		}
	}
    #endregion

    private void OnOpen() {
		gameObject.SetActive(true);
		_postBtn.interactable = true;
	}

	private void OnEnable() {
		Scroll.OnFill += OnFillItem;
		Scroll.OnHeight += OnHeightItem;
		Scroll.OnPull += OnPullItem;
	}

    private void OnDisable() {
		Scroll.OnFill -= OnFillItem;
		Scroll.OnHeight -= OnHeightItem;
		Scroll.OnPull -= OnPullItem;
	}

    private void OnRemoveItem(int index) {
		Scroll.Recycle(index);
	}
}
