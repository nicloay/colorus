using UnityEngine;
using System.Collections;

namespace colorpicker{
	public class GetValueFromColorPicker : MonoBehaviour {
		ColorPicker colorPicker=new ColorPicker();
		
		Rect paletteRect = new Rect(0,0,120,120);
		Rect sliderRect  = new Rect(130,0,20,120);
		
		Rect textRect = new Rect(0,130,160,20);
		
		Color32 colorValue;
		
		void Start () {
			colorPicker.initialize(paletteRect,sliderRect);
		}
				
		void OnGUI () {
			colorValue = colorPicker.OnGUI();
			GUI.TextField(textRect, colorValue.ToString());			
		}
	}	
}
