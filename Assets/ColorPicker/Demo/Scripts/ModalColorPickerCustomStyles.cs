using UnityEngine;
using System.Collections;

namespace colorpicker{
	public class ModalColorPickerCustomStyles : MonoBehaviour {
		public Texture2D activeColorPointerTexture; //Custom pointer texture
		public GUIStyle sliderThumbStyle;           //custom right slider thumb
		public string windowCaption = "Modal ColorPicker with customstyles";
		#region setup Rects
		Rect modalWindowRect = new Rect(10,260,280,240);
		Rect palleteRect = new Rect(10,20,200,200);
		Rect sliderRect = new Rect(220, 20, 40, 200);
		Rect closeModalWindowButtonRect = new Rect(260,0,20,20);
		#endregion
		public ColorPicker colorPicker = new ColorPicker();
		
		bool modalEnabled = false;
		
		void Awake(){
			colorPicker.initialize(palleteRect,sliderRect,activeColorPointerTexture,sliderThumbStyle);
		}
		
		void OnGUI(){
			GUI.matrix = ScaleSetup.instance.scaleMatrix;
			if (modalEnabled){
				modalWindowRect = GUI.ModalWindow(1,modalWindowRect,doMyModalWindow,windowCaption);				
			}else{
				Rect buttonRect = new Rect(modalWindowRect);
				buttonRect.height = 40;
				if (GUI.Button(buttonRect,"show "+windowCaption))
					modalEnabled = true;
			}
			GUI.matrix = Matrix4x4.identity;
		}
		
		void doMyModalWindow(int windowId){
			if (GUI.Button(closeModalWindowButtonRect,"x"))
				modalEnabled = false;
			colorPicker.OnGUI();
			GUI.DragWindow();
		}
		
	}	
}
