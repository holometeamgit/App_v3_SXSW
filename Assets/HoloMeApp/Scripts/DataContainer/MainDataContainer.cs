using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MainDataContainer : MonoBehaviour {

    public TaggableDataContainer<string, ITaggable<string>> Container {
        get {
            if (dataContainer == null)
                dataContainer = new TaggableDataContainer<string, ITaggable<string>>();
            return dataContainer;
        }
    }

    private TaggableDataContainer<string, ITaggable<string>> dataContainer;
}
