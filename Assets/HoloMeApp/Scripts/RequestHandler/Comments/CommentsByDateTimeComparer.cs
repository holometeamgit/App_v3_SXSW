using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.Content;

public class CommentsByDateTimeComparer : IComparer<CommentJsonData> {
    #region IComparer<CommentJsonData?> Members

    public int Compare(CommentJsonData x, CommentJsonData y) {
        return CompareDate(x?.created_at, y?.created_at);
    }

    private int CompareDate(DateTime? x, DateTime?y) {
        DateTime nx = x ?? DateTime.MaxValue;
        DateTime ny = y ?? DateTime.MaxValue;

        return nx.CompareTo(ny);
    }

    #endregion
}