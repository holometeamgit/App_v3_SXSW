using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ProductJsonData
{
    public List<ProductElement> products;

    public ProductJsonData()
    {
        products = new List<ProductElement>();
    }

    [Serializable]
    public class ProductElement
    {
        public string name;
        public string product_id;
        public float price;
        public DateTime condition_start_date;
        public DateTime condition_end_date;
    }
}
