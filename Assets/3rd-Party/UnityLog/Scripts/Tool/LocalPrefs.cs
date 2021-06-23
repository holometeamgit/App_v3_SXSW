using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Beem.Utility.UnityConsole {

    /// <summary>
    /// Local Prefs. Work without saving from session to session
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LocalPrefs<T> {
        private static Dictionary<string, T> prefs = new Dictionary<string, T>();

        public static void Set(string key, T value) {
            if (prefs.ContainsKey(key)) {
                prefs[key] = value;
            } else {
                prefs.Add(key, value);
            }
        }

        public static T Get(string key, T defaultValue) {
            if (!prefs.ContainsKey(key)) {
                prefs[key] = defaultValue;
            }
            return prefs[key];
        }

        public static void DeleteKey(string key) {
            if (prefs.ContainsKey(key)) {
                prefs.Remove(key);
            }
        }
    }
}
