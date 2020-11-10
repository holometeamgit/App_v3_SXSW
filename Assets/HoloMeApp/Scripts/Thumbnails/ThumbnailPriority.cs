/*
 * contains the priority of the data that is requested from the server
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ThumbnailPriority
{
    //descending stages[0] > stages[1]
    public List<StreamJsonData.Data.Stage> Stages;
}
