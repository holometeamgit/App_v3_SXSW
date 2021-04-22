using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.Content;

public class CommentsByIDComparer : IComparer<CommentJsonData> {
    #region IComparer<CommentJsonData?> Members

    public CommentsByIDComparer() { }
    public CommentsByIDComparer(bool isReverse) {
        _isReverse = isReverse;
    }

    bool _isReverse;

    public int Compare(CommentJsonData x, CommentJsonData y) {
        return CompareDate(x?.id, y?.id);
    }

    private int CompareDate(int? x, int? y) {
        int nx = x ?? int.MaxValue;
        int ny = y ?? int.MaxValue;

        if (_isReverse) {
            return ny.CompareTo(nx);
        } else {
            return nx.CompareTo(ny);
        }
    }

    #endregion
}