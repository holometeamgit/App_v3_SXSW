using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class СolorRepeater : MaskableGraphic
{
	[SerializeField] List<MaskableGraphic> targetGraphics;
	public override Color color {
		set {
			foreach(var element in targetGraphics) {
				element.color = value;
			}
        }
	}

}
