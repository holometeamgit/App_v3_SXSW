using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIThumbnailsController : MonoBehaviour {
    [SerializeField] MediaFileDataHandler mediaFileDataHandler;
    [SerializeField] GameObject btnThumbnailPrefab;
    [SerializeField] Transform content;

    Dictionary<long, ThumbnailElement> thumbnailElementsDictionary;

    Dictionary<long, BtnThumbnailItemV2> btnThumbnailItemsDictionary;
    List<BtnThumbnailItemV2> btnThumbnailItems;

    List<StreamJsonData.Data> dataList;

    StreamDataEqualityComparer streamDataEqualityComparer;

    public void SetStreamJsonData(List<StreamJsonData.Data> data) {
        dataList = data;
    }

    public void UpdateData() {
        PrepareBtnThumbnails();//приготовил кнопки
        PrepareThumbnailElement();//приготовил данные

        //подготовил быстрое обращение
        for (int i = 0; i < dataList.Count; i++) {
            btnThumbnailItemsDictionary[dataList[i].id] = btnThumbnailItems[i];
        }

        //для каждой кнопки теперь нужно обновить данные TODO
    }

    public void UpdateThumbnailElement(long id) {

    }

    public void RemoveThumbnailElement(long id) {

    }

    private void Awake() {
        thumbnailElementsDictionary = new Dictionary<long, ThumbnailElement>();
        btnThumbnailItemsDictionary = new Dictionary<long, BtnThumbnailItemV2>();
        btnThumbnailItems = new List<BtnThumbnailItemV2>();
    }

    private void PrepareBtnThumbnails() {
        int quantityDifference = btnThumbnailItems.Count - dataList.Count;

        if (quantityDifference > 0) {
            for (int i = dataList.Count; i < btnThumbnailItems.Count; i++) {
                btnThumbnailItems[i].Deactivate();
            }
        } else {
            for (int i = 0; i < btnThumbnailItems.Count; i++) {
                btnThumbnailItems[i].Activate();
            }
            for (int i = 0; i < quantityDifference; i++) {
                GameObject btnThumbnailItemsGO = Instantiate(btnThumbnailPrefab, content);
                BtnThumbnailItemV2 btnThumbnailItem = btnThumbnailItemsGO.GetComponent<BtnThumbnailItemV2>();
                btnThumbnailItems.Add(btnThumbnailItem);
            }
        }
    }

    private void PrepareThumbnailElement() {
        foreach (var thumbnailData in dataList) {
            if (thumbnailElementsDictionary.ContainsKey(thumbnailData.id)) {
                ThumbnailElement thumbnailElement = thumbnailElementsDictionary[thumbnailData.id];
                if (thumbnailElement.Data == thumbnailData || streamDataEqualityComparer.Equals(thumbnailElement.Data, thumbnailData))
                    continue;
            }
            thumbnailElementsDictionary[thumbnailData.id] = new ThumbnailElement(thumbnailData, mediaFileDataHandler);
        }
    }

    //у нас есть отсортированный список данных dThu и нужен список отсортированных thu и есть просто список thuObj
    //элемент из thu это не только сухие данные, но и текстура 
    //получаем новый список данных  dThu - это тот же, что и раньше, но мы слушаем событие обновления (создать событие в ThumbnailsDataContainer, что данные все необходимые загрузились)
    //проверяем размер  списка thuObj и для этого размера создаём нужное колличество нам thumbnials или включаем отключённые, если не хватает. если много, то отключаем лишние 
    //идёи подряд по списку данных сухие, если есть в thu, то ставим на пазицию нужную, если нет , то добавляем в промежуточную нужную позицию новый элемент и запускаем процесс загрузки
    //назначаем подряд thuObj свой элемент из thu. Элементы сами смотрят в хватает нужных данных или нет. Если нет, то грузаят заново сами. Проверяют при каждой активации


    //по ThumbnailsDataContainer {
    //public Action<long> OnStreamUpdated;
    //public Action<long> OnStreamRemoved;
    //мы знаем в нашем списке thumbnail и какие удалились, что они обновились и нужно пробежавшить по списку.
    //  Если обновились, то в элемент из thu добавляются данные из dThu, но текстура удаляется,
    //  если она не совпадает (это должен быть метод в thuElement, который сам умеет обновлять свои данные)
}

public class ThumbnailElement {
    public long Id { get; }
    public StreamJsonData.Data Data { get; }
    public Texture texture;

    public Action<Texture> OnTextureLoaded;
    public Action OnErrorTextureLoaded;

    public ThumbnailElement(StreamJsonData.Data data, MediaFileDataHandler mediaFileDataHandler) {
        Data = data;
        Id = data.id;

        mediaFileDataHandler.LoadImg(Data.preview_s3_url,
                                     FetchTexture,
                                     ErrorFetchTexture);
    }

    private void FetchTexture(long code, string body, Texture texture) {
        this.texture = texture;
        OnTextureLoaded?.Invoke(texture);
    }
    private void ErrorFetchTexture(long code, string body) {
        OnErrorTextureLoaded?.Invoke();
    }

}