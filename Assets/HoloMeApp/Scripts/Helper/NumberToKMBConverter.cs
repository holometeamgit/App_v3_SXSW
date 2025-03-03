﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

/// <summary>
/// NumberToKMBConverter can convert numbers specific format
/// 12.4K
/// 12.45M
/// 12.456B
/// </summary>
public static class NumberToKMBConverter {
    public static string ToKMB(this decimal num) {
        if (num > 999999999 || num < -999999999) {
            return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
        } else
        if (num > 999999 || num < -999999) {
            return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
        } else
        if (num > 999 || num < -999) {
            return num.ToString("0,.#K", CultureInfo.InvariantCulture);
        } else {
            return num.ToString(CultureInfo.InvariantCulture);
        }
    }
}
