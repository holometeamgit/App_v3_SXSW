using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beem.Utility.UnityConsole
{

    /// <summary>
    /// DateTimePrefs
    /// </summary>
    public static class DateTimePrefs
    {
        public static void Set(string key, DateTime dateTime) {
            PlayerPrefs.SetInt("Year" + key, dateTime.Year);
            PlayerPrefs.SetInt("Month" + key, dateTime.Month);
            PlayerPrefs.SetInt("Day" + key, dateTime.Day);
            PlayerPrefs.SetInt("Hour" + key, dateTime.Hour);
            PlayerPrefs.SetInt("Minute" + key, dateTime.Minute);
            PlayerPrefs.SetInt("Second" + key, dateTime.Second);
            PlayerPrefs.SetInt("Millisecond" + key, dateTime.Millisecond);
            PlayerPrefs.Save();
        }

        public static DateTime Get(string key) {
            int year = PlayerPrefs.GetInt("Year" + key, DateTime.MinValue.Year);
            int month = PlayerPrefs.GetInt("Month" + key, DateTime.MinValue.Month);
            int day = PlayerPrefs.GetInt("Day" + key, DateTime.MinValue.Day);
            int hour = PlayerPrefs.GetInt("Hour" + key, DateTime.MinValue.Hour);
            int minute = PlayerPrefs.GetInt("Minute" + key, DateTime.MinValue.Minute);
            int second = PlayerPrefs.GetInt("Second" + key, DateTime.MinValue.Second);
            int millisecond = PlayerPrefs.GetInt("Millisecond" + key, DateTime.MinValue.Millisecond);
            DateTime date = new DateTime(year, month, day, hour, minute, second, millisecond);
            return date;
        }

        public static void DeleteKey(string key) {
            PlayerPrefs.DeleteKey("Year" + key);
            PlayerPrefs.DeleteKey("Month" + key);
            PlayerPrefs.DeleteKey("Day" + key);
            PlayerPrefs.DeleteKey("Hour" + key);
            PlayerPrefs.DeleteKey("Minute" + key);
            PlayerPrefs.DeleteKey("Second" + key);
            PlayerPrefs.DeleteKey("Millisecond" + key);
            PlayerPrefs.Save();
        }
    }
}
