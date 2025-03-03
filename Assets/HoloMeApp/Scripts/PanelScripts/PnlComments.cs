﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beem.UI;
using Beem.Content;
using System;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// pnl comments
/// </summary>
public class PnlComments : MonoBehaviour {
    public delegate CommentJsonData GetItemByOrdinalID(int index);
    public event GetItemByOrdinalID onGetItemByOrdinalIndex;
    public Action onRefresh;
    public Action onLoadNext;
    public Action<int> onOpen;
    public Action onClose;
    public Action<string> onPost;

    [SerializeField]
    private InfiniteScroll _scroll;

    [SerializeField]
    Animator _animator;
    [SerializeField]
    TMP_Text _commentsCount;

    private bool _isCanOpen;
    private bool _afterRefresh;
    private UICommentElement uiCommentElementPrefab;

    private const string COUNT_COMMENTS = " comments";
    private const string ONE_COMMENT = " comment";
    private const string NO_COMMENTS = "no comments";

    #region UI

    /// <summary>
    /// open comments for content with id
    /// </summary>
    public void OpenComments(int contentId) {
        HelperFunctions.DevLog("pnlcomments OpenComments " + contentId);
        PrepareToShowComments();
        _isCanOpen = true;
        _afterRefresh = true;
        OnOpen();
        uiCommentElementPrefab = _scroll.Prefab.GetComponent<UICommentElement>();
        _scroll.IsPullBottom = true;
        onOpen?.Invoke(contentId);
    }

    /// <summary>
    /// send comment to serber for current content 
    /// </summary>
    public void Post(string text) {
        if (string.IsNullOrWhiteSpace(text))
            return;
        onPost?.Invoke(text);
    }

    /// <summary>
    /// close comments
    /// </summary>
    public void CloseComments() {
        _isCanOpen = false;
        HidePost();
        _animator.SetBool("IsShowComment", _isCanOpen);
        StreamCallBacks.onCommentsClosed?.Invoke();
    }

    /// <summary>
    /// disable and cleare on close event
    /// </summary>
    public void OnClose() {
        gameObject.SetActive(false);
        _scroll.RecycleAll();
    }

    /// <summary>
    /// prepare to show comments
    /// </summary>
    public void PrepareToShowComments() {
        _isCanOpen = true;
        _animator.SetBool("IsShowComment", _isCanOpen);
    }

    /// <summary>
    /// Hide Post 
    /// </summary>
    public void HidePost() {
        _animator.SetBool("IsPostState", false);
        _scroll.IsPullBottom = true;
        _scroll.IsPullTop = true;
    }

    /// <summary>
    ///  Show Post 
    /// </summary>
    public void ShowPost() {
        _animator.SetBool("IsPostState", true);
        _scroll.IsPullBottom = false;
        _scroll.IsPullTop = false;
    }
    #endregion

    #region count comments
    /// <summary>
    ///  do update on refresh event
    /// </summary>
    public void OnRefreshUpdateCommentsCount(int count) {
        if (count == 0) {
            _commentsCount.text = NO_COMMENTS;
        } else if (count == 1) {
            _commentsCount.text = 1 + ONE_COMMENT;
        } else {
            _commentsCount.text = count + COUNT_COMMENTS;
        }
    }
    #endregion

    #region response from model
    /// <summary>
    /// calls after posted to backend
    ///  do refresh on post event
    /// </summary>
    public void OnPost() {
        Refresh();
        HidePost();
    }

    /// <summary>
    /// calls after didn't post to backend
    /// </summary>
    public void OnFailPost() {
        //TODO show msg for user
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="count">count comments</param>
    /// <param name="pullCount">new loaded comments count</param>
    public void OnApplyDataTo(int count, int pullCount) {
        if (!_isCanOpen)
            return;

        if (!gameObject.activeSelf)
            OnOpen();

        _scroll.ApplyDataTo(count, pullCount, InfiniteScroll.Direction.Bottom);

        if (_afterRefresh) {
            _afterRefresh = false;
            _scroll.MoveToSide(InfiniteScroll.Direction.Top);
        }
    }


    /// <summary>
    /// on All Data Loaded 
    /// hide btn
    /// </summary>
    public void OnAllDataLoaded() {
        _scroll.IsPullBottom = false;
    }

    #endregion

    #region response from ui

    void OnFillItem(int index, GameObject item) {
        CommentJsonData commentData = onGetItemByOrdinalIndex(index);
        item.GetComponentInChildren<UICommentElement>().UpdateData(commentData.user, commentData.body, commentData.CreatedAt, 0, commentData.id);
    }

    int OnHeightItem(int index) {
        CommentJsonData commentData = onGetItemByOrdinalIndex(index);

        return Mathf.CeilToInt(uiCommentElementPrefab.GetRequiredHeight(commentData.user, commentData.body));
    }

    void OnPullItem(InfiniteScroll.Direction direction) {
        if (direction == InfiniteScroll.Direction.Top) {
            Refresh();
        } else {
            onLoadNext?.Invoke();
        }
    }
    #endregion

    private void Refresh() {
        _afterRefresh = true;
        onRefresh?.Invoke();
        _scroll.IsPullBottom = true;
    }

    private void OnOpen() {
        gameObject.SetActive(true);
    }

    private void OnEnable() {
        _scroll.OnFill += OnFillItem;
        _scroll.OnHeight += OnHeightItem;
        _scroll.OnPull += OnPullItem;
    }

    private void OnDisable() {
        _scroll.OnFill -= OnFillItem;
        _scroll.OnHeight -= OnHeightItem;
        _scroll.OnPull -= OnPullItem;
    }

    private void OnRemoveItem(int index) {
        _scroll.Recycle(index);
    }
}
