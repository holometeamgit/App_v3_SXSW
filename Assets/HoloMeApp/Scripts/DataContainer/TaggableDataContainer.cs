/*
 * The class refines the container of elements.
 * Items are stored by key.
 * The key stores a list of elements of type ITaggable <K>
 * The list of items can only store items of different types.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggableDataContainer<K, T> where T: ITaggable<K> {

    Dictionary<K, List<T>> taggableObjectDictionary;

    public TaggableDataContainer() {
        taggableObjectDictionary = new Dictionary<K, List<T>>();
    }

    public ST Get<ST>(K key) where ST : T {
        if (!taggableObjectDictionary.ContainsKey(key))
            return default;

        var elements = taggableObjectDictionary[key];

        return elements == default ? default : GetElementByType<ST>(elements);
    }

    public void Add<ST>(ST taggableElement) where ST : T {
        if (!taggableObjectDictionary.ContainsKey(taggableElement.Key))
            taggableObjectDictionary[taggableElement.Key] = new List<T>();

        var taggableElements = taggableObjectDictionary[taggableElement.Key];

        //remove same type element
        Remove<ST>(taggableElement.Key);

        taggableElements.Add(taggableElement);
    }

    public void Add<ST>(List<ST> taggableElements) where ST : T {
        if (taggableElements == default)
            return;

        foreach (var element in taggableElements) {
            Add(element);
        }
    }

    //does a specific item contain?
    public bool Contains(T taggableElement) {
        if (!taggableObjectDictionary.ContainsKey(taggableElement.Key))
            return false;

        var elements = taggableObjectDictionary[taggableElement.Key];
        if (elements == null)
            return false;

        return elements.Contains(taggableElement);
    }

    public bool ContainsType<ST>(K key) where ST : T {
        if (!taggableObjectDictionary.ContainsKey(key))
            return false;

        var elements = taggableObjectDictionary[key];

        if (elements == default)
            return false;

        return GetElementByType<ST>(elements) != null;
    }

    public void Remove<ST>(K key) where ST : T {
        if (!taggableObjectDictionary.ContainsKey(key))
            return;

        var elements = taggableObjectDictionary[key];

        if (elements == default)
            return;

        foreach(var element in elements) {
            if (element is ST) {
                elements.Remove(element);
                return;
            }
        }
    }

    public void Remove<ST>(List<ST> taggableElements) where ST : T {
        if (taggableElements == null)
            return;

        foreach(var element in taggableElements) {
            Remove<ST>(element.Key);
        }
    }

    private ST GetElementByType<ST>(List<T> elements) where ST : T {
        if (elements == null)
            return default;

        foreach (var element in elements) {
            if (element is ST)
                return (ST)element;
        }
        return default;
    }
}