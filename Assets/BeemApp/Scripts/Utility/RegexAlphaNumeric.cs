using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Regex AlphaNumber
/// </summary>
public class RegexAlphaNumeric {
    /// <summary>
    /// Regex AlphaNumeric
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string RegexResult(string s) {
        string tempS = string.Empty;
        for (int i = 0; i < s.Length; i++) {
            if (Regex.IsMatch(s[i].ToString(), @"^[a-zA-Z0-9]+$")) {
                tempS += s[i];
            }
        }
        return tempS;
    }
}
