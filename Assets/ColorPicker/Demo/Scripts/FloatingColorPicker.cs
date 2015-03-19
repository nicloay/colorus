using UnityEngine;
using System.Collections;

namespace colorpicker{
	public class FloatingColorPicker : MonoBehaviour {
		public ColorPicker colorPicker = new ColorPicker();		
		
		Rect windowRect = new Rect(310,10,270,320);
		
		Rect palleteRect = new Rect(10,20,200,200);
		Rect sliderRect = new Rect(220, 20, 40, 200);
		
		Rect rectR = new Rect(10, 230, 50,20);
		Rect rectG = new Rect(10, 260, 50,20);
		Rect rectB = new Rect(10, 290, 50,20);
		
		Rect rectSliderR = new Rect(70, 230, 50,20);
		Rect rectSliderG = new Rect(70, 260, 50,20);
		Rect rectSliderB = new Rect(70, 290, 50,20);
		
		Rect rectH = new Rect(210, 230,50,20);
		Rect rectS = new Rect(210, 260,50,20);
		Rect rectV = new Rect(210, 290,50,20);
		
		Rect currentColorRect = new Rect(130,230,70,70);	
		GUIStyle _currentColorStyle;
		GUIStyle currentColorStyle{
			get{
				if (_currentColorStyle==null){
					_currentColorStyle = new GUIStyle();
					_currentColorStyle.normal.background = TextureUtil.createEmptyTexture(70,70,Color.white);					
				}
				return _currentColorStyle;
			}
		}
		
		void Awake(){
			colorPicker.initialize(palleteRect,sliderRect);
		}
	
		void OnGUI(){
			GUI.matrix = ScaleSetup.instance.scaleMatrix;
			windowRect=GUI.Window(0,windowRect,doMyWindow,"Floating Window Colorpicker");
			GUI.matrix = Matrix4x4.identity;
		}
	
	
		void doMyWindow (int windowId)
		{			
			Color32 color= colorPicker.OnGUI();		
			GUI.TextField(rectR,"R "+color.r.ToString("000"));
			GUI.TextField(rectG,"G "+color.g.ToString("000"));
			GUI.TextField(rectB,"B "+color.b.ToString("000"));
			
			byte newR = (byte)GUI.HorizontalSlider(rectSliderR,color.r,0,255);
			byte newG = (byte)GUI.HorizontalSlider(rectSliderG,color.g,0,255);
			byte newB = (byte)GUI.HorizontalSlider(rectSliderB,color.b,0,255);
			if (newR!=color.r || newG!=color.g || newB!=color.b){
				colorPicker.setRGBColor(new Color32(newR,newG,newB,1));			
			}
			
			HSVColor hsv= colorPicker.getHSV();
			GUI.TextField(rectH,"H "+hsv.h.ToString("0.000"));
			GUI.TextField(rectS,"S "+hsv.s.ToString("0.000"));
			GUI.TextField(rectV,"V "+hsv.v.ToString("0.000"));
			
			Color32 backupColor = GUI.color;
			GUI.backgroundColor =colorPicker.getRGB();
			GUI.Box(currentColorRect, GUIContent.none, currentColorStyle);
			GUI.color = backupColor;
			
			GUI.DragWindow();
			
		}
	}
}