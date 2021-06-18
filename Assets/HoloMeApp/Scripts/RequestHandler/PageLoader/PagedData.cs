using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Beem.Pagination {

    [Serializable]
    public class PagedData<T> {
        /// <summary>
        /// TotalItems
        /// </summary>
        public int count;
        /// <summary>
        /// Link to next page
        /// </summary>
        public string next;
        /// <summary>
        /// Link to prev page
        /// </summary>
        public string previous;
        public List<T> results;
    }
}