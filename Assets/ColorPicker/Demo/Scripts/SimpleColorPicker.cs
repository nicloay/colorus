using UnityEngine;
using System.Collections;

namespace colorpicker{
	public class SimpleColorPicker : MonoBehaviour {		
		public ColorPicker colorPicker = new ColorPicker();
		
		Rect palleteRect = new Rect(10, 50,200,200);
		Rect sliderRect = new Rect(220, 50, 40, 200);				
		
		void Awake () {			
			colorPicker.initialize(palleteRect,sliderRect);							
		}		
		
		void OnGUI () {			
			colorPicker.OnGUI();									
		}	
	}	
}
