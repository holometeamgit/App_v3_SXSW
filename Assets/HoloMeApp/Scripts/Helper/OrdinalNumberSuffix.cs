using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Beem {
    public class OrdinalNumberSuffix : MonoBehaviour {

        public static string AddOrdinalNumberSuffixDat(int day) {
            switch (day) {
                case 1:
                case 21:
                case 31:
                    return day + "st";
                case 2:
                case 22:
                    return day + "nd";
                case 3:
                case 23:
                    return day + "rd";
                default:
                    return day + "th";
            }
        }
    }
}