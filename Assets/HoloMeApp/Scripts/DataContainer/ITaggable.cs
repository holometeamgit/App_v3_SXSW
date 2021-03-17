using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITaggable<T> {
    T Key { get; }
}
