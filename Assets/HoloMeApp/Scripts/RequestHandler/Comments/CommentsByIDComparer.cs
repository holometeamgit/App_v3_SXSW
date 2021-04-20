using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Beem.Content;

public class CommentsByIDComparer : IComparer<CommentJsonData> {
    #region IComparer<CommentJsonData?> Members

    public int Compare(CommentJsonData x, CommentJsonData y) {
        return CompareDate(x?.id, y?.id);
    }

    private int CompareDate(int? x, int? y) {
        int nx = x ?? int.MaxValue;
        int ny = y ?? int.MaxValue;

        return nx.CompareTo(ny);
    }

    #endregion
}