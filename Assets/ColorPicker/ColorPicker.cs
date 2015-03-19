using UnityEngine;
using System.Collections;


namespace colorpicker{	
	
	[System.Serializable]
	public class ColorPicker {
		public GUIStyle sliderStyle;
		public GUIStyle sliderThumbStyle;
		bool baseHue = false;


		Rect paletteRect;
		Rect hueSliderRect;
		Texture2D sliderTexture;
		Texture2D paletteTexture;
		Texture2D activeColorPointerTexture;
		
		Color32 currentRGBColor;
		HSVColor currentHSVColor;
		int currentHue = 0;
		Rect activeColorPosition;
		bool receviedPosition = false;
		
		
		public void initialize(Rect paletteRect, Rect hueSliderRect, Texture2D pointerTexture = null, GUIStyle sliderThumbStyle = null){
			receviedPosition = true;
			this.paletteRect = paletteRect;
			paletteTexture = new Texture2D((int)paletteRect.width, (int)paletteRect.height, TextureFormat.ARGB32, false);
			sliderTexture =  new Texture2D((int)hueSliderRect.width, (int)hueSliderRect.height, TextureFormat.ARGB32, false);
			if (pointerTexture == null){
				activeColorPointerTexture = TextureUtil.createFrame(9,9,2,Color.white,Color.clear);				
			} else {
				activeColorPointerTexture = pointerTexture;
			}
			
			if (sliderThumbStyle==null)
				this.sliderThumbStyle = getSliderThumbStyle ();
			else
				this.sliderThumbStyle = sliderThumbStyle;
			
			updateSliderTexture((int)hueSliderRect.width, (int)hueSliderRect.height);
			int thumbHeight = this.sliderThumbStyle.normal.background.height;
			int halfThumbHeight = thumbHeight/2;
			hueSliderRect.y -=halfThumbHeight;
			hueSliderRect.height +=thumbHeight;
			this.hueSliderRect = hueSliderRect;
			if (sliderStyle==null)
				sliderStyle = new GUIStyle();								
			sliderStyle.normal.background = sliderTexture;
			sliderStyle.overflow = new RectOffset(0,0, -halfThumbHeight,-halfThumbHeight );
				
			this.hueSliderRect = hueSliderRect;
			updatePaletteTexture();
			calculateActiveColorPositionAndUpdateColors(new Vector2( paletteRect.x, paletteRect.y));			
		}
		
		Vector2 lastPosition;
		
		void calculateActiveColorPositionAndUpdateColors(Vector2 screenPosition){
			calculateActiveColorPosition(screenPosition);
			recalculateHsvAndRGB();
		}
		
		void calculateActiveColorPosition(Vector2 screenPosition){
			
			screenPosition.x = Mathf.Clamp(screenPosition.x, paletteRect.x, paletteRect.x + paletteRect.width);
			screenPosition.y = Mathf.Clamp(screenPosition.y, paletteRect.y, paletteRect.y + paletteRect.height);
			
			
			lastPosition = screenPosition;
			activeColorPosition = new Rect(screenPosition.x - activeColorPointerTexture.width / 2,
										   screenPosition.y - activeColorPointerTexture.height / 2,
										   activeColorPointerTexture.width,
										   activeColorPointerTexture.height);


		}
		void recalculateHsvAndRGB(){
			currentHSVColor = getHSV();
			currentRGBColor = getRGB(currentHSVColor);			
		}		

		public Color32 OnGUI(){
			if (receviedPosition){
				GUI.DrawTexture(paletteRect,paletteTexture);
				GUI.DrawTexture(activeColorPosition, activeColorPointerTexture);
				int newHue =(int) GUI.VerticalSlider(hueSliderRect, currentHue, hueSliderRect.height-1, 0, sliderStyle,sliderThumbStyle);
				if (newHue!=currentHue){
					currentHue = newHue;
					recalculateHsvAndRGB();
					updatePaletteTexture();					
				}
				handleEvent();
			}
			return currentRGBColor;
		}
		
		bool mouseInMotion = false;
		
		void handleEvent(){			
			Event e = Event.current;
				
			if (GUIUtility.hasModalWindow && mouseInMotion && e.type == EventType.Ignore){			
				mouseInMotion = false;	
			} 
			
			if (e.type == EventType.MouseDown && paletteRect.Contains(e.mousePosition)){
				calculateActiveColorPositionAndUpdateColors(e.mousePosition);	
				mouseInMotion = true;
				e.Use();
			} else if (mouseInMotion && e.type == EventType.MouseUp){		
				calculateActiveColorPositionAndUpdateColors(e.mousePosition);
				mouseInMotion = false;			
				e.Use();
			}
			
    			if (mouseInMotion && e.type == EventType.mouseDrag){
				calculateActiveColorPositionAndUpdateColors(e.mousePosition);	
				e.Use();
			}
		}
		Color32[] colors;
		void updatePaletteTexture(){
			float width = paletteRect.width;
			float height = paletteRect.height;
			if (colors ==null)
				colors= new Color32[(int)(width * height)];
			float h,s,v;

			int H;
			float f,p,q,t,r,g,b;
			float value = (float)currentHue / hueSliderRect.height;
			int current=0;
			for (float heightI = 0; heightI < height; heightI++) {
				for (float widthI = 0; widthI < width; widthI++) {
					if (baseHue){
						h = value;
						s = widthI / width;
						v = heightI / height;
					} else {
						h = widthI/width;
						s = heightI/height;
						v = value;
					}
					

					H = (int)(h * 6);
					
					f = h * 6 - H;
					p = v * (1 - s);
					q = v * (1 - f * s);
					t = v * (1 - (1 - f) * s);
					
					r=0;
					g=0;
					b=0;
					
					switch (H) {
					case 0:
						r = v; g = t; b = p;
						break;	
					case 1:
						r = q; g = v; b = p;
						break;
					case 2:
						r = p; g = v; b = t;
						break;
					case 3:
						r = p; g = q; b = v;
						break;
					case 4:
						r = t; g = p; b = v;
						break;
					case 5:
						r = v; g = p; b = q;
						break;
					}
					
					
					
					colors[current].r = (byte)(255*r);
					colors[current].g = (byte)(255*g);
					colors[current].b = (byte)(255*b);
					colors[current].a = 255;
					current++;
				}
			}
			paletteTexture.SetPixels32(colors);
			paletteTexture.Apply();				
		}
		
		
		void updateSliderTexture (int width, int height){			
			Color32[] colors= new Color32[width * height];
			float val;
			Color32 color;
			int curent=0;
			
			for (int i = 0; i < height; i++) {
				val = (float)i / height;
				if (baseHue)
					color = ColorUtil.hsvToRgb( val, 1.0f, 1.0f);
				else 
					color = ColorUtil.hsvToRgb(0,0,val);

				for (int u = 0; u < width; u++) {					
					colors[curent++]=color;
				}
			}		
			sliderTexture.SetPixels32(colors);
			sliderTexture.Apply();
		}
		
		public HSVColor getHSV(){
			float x = lastPosition.x -  paletteRect.x;
			float y = lastPosition.y - paletteRect.y;
			HSVColor c = new HSVColor();
			c.h = (float)currentHue / hueSliderRect.height;
			c.s = x / paletteRect.width;
			c.v = 1 - y / paletteRect.height;
			return c;
		}
		
		public Color32 getRGB(HSVColor c = null){			
			return ColorUtil.hsvToRgb(c==null ? currentHSVColor : c);			
		}
		
		public void setColor(Color32 color){
			setRGBColor(color);
		}
		public void setRGBColor(Color32 color){
			float h,s,v;
			currentRGBColor = color;
			if (baseHue)
				ColorUtil.RGBToHSV((Color)color,out h,out s,out v);
			else 
				ColorUtil.RGBToHSV((Color)color,out s,out v,out h);

			currentHSVColor = new HSVColor(h,s,v);			
			currentHue = (int)(hueSliderRect.height * h);
			
			updatePaletteTexture();
			calculateActiveColorPosition(new Vector2(paletteRect.x + paletteRect.width  * s,
													 paletteRect.y + paletteRect.height * (1 - v)));	
		}
		
		public GUIStyle getSliderThumbStyle ()
		{
			GUIStyle sliderThumbStyle = new GUIStyle();
			Texture2D thumbTexture = TextureUtil.createFrame(9,9,3,Color.white,Color.clear);
			sliderThumbStyle.normal.background = thumbTexture;			
			sliderThumbStyle.border = new RectOffset(4,4,0,0);
			sliderThumbStyle.overflow = new RectOffset(3,3,0,0);
			sliderThumbStyle.alignment = TextAnchor.MiddleCenter;
			sliderThumbStyle.fixedHeight = 10;	
			return sliderThumbStyle;
		}
	}	
}

