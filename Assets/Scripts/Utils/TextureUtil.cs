using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class TextureUtil {
	public static Color32 SOLID_COLOR        = new Color32 (255, 255, 255, 255)           ;
	public static Color32 EMPTY_COLOR        = new Color32 (0, 0, 0, 0)                   ;
	public static string  ASSET_TEXTURE_HOME = "Assets/Resources/Textures/ServiceLayers/" ;
	
	public static Texture2D createAndSaveAlpha8Texture (int width, int height, Color color, string name) {
#if UNITY_EDITOR
		string assetPath = ASSET_TEXTURE_HOME + name + ".png";
		return createAndSaveTexture (width, height, color, assetPath, TextureFormat.Alpha8, TextureImporterFormat.Alpha8);
#else
		return null;
#endif

	}
	
	public static Texture2D createAndSaveAlphaRGBA32Texture (int width, int height, Color color, string name) {
#if UNITY_EDITOR
		string assetPath = ASSET_TEXTURE_HOME + name + ".png";
		return createAndSaveTexture (width, height, color, assetPath, TextureFormat.RGBA32, TextureImporterFormat.RGBA32);
#else
		return null;
#endif
	}
	
	
#if UNITY_EDITOR
	public static Texture2D createAndSaveTexture (int width, int height, Color color, string assetPath, TextureFormat textureFormat, TextureImporterFormat textureImporterFormat)
	{
		Texture2D texture=createTexture(width,height, color,textureFormat);		

		texture=saveTexture2DToAssets(texture, assetPath);
		texture=importTexture (assetPath, textureImporterFormat,Mathf.Max(width,height));		
		return texture;
	}
#endif

	
	
	public static Texture2D savePixelArrayToFile (int width, int height, Color32[] pixels, string path,  bool importToAssetDatabase=true) {
		Texture2D tex = new Texture2D (width, height, TextureFormat.RGBA32, false);
		tex.SetPixels32 (pixels);
		tex.Apply ();
		
		return saveTexture2DToAssets (tex, path, importToAssetDatabase);
	}
	
	public static Texture2D createTexture (int width, int height, Color color, TextureFormat textureFormat) {
		
		Texture2D tex = new Texture2D (width, height, textureFormat, false);
		Color32[] pixels = tex.GetPixels32 ();
		for (int i=0; i<pixels.Length; i++) {
			pixels [i] = color;	
		}			
		tex.SetPixels32 (pixels);
		tex.Apply (false);		
		return tex;
	}
	
	public static Texture2D saveTexture2DToAssets (Texture2D texture, string assetPath, bool importToAssetDatabase=true) {		
#if UNITY_WEBPLAYER
	Debug.LogWarning("texture can't be saved in webplayer build mode, please switch to standalone version");	
#endif
		
		if (!assetPath.StartsWith ("Assets/"))
			assetPath = "Assets/" + assetPath;
		if (!assetPath.EndsWith ("png"))
			assetPath = assetPath + ".png";
#if UNITY_EDITOR && !UNITY_WEBPLAYER	
		(new FileInfo(assetPath)).Directory.Create();
#endif
		byte[] bytes = texture.EncodeToPNG ();
		if (bytes != null) {
#if !UNITY_WEBPLAYER
			File.WriteAllBytes (assetPath, bytes);			
#endif			
		}		
		//to prevent leaking, remove this texture and import it again
		Object.DestroyImmediate ((Object)texture);
#if UNITY_EDITOR
		if (importToAssetDatabase)
			AssetDatabase.ImportAsset(assetPath);		
#endif
		return texture;
	}
	
	public static void updateTextureAsset (Texture2D texture) {
#if UNITY_EDITOR
		byte[] bytes = texture.EncodeToPNG();
		string assetPath= AssetDatabase.GetAssetPath(texture.GetInstanceID());
		FileStream file=File.Open(assetPath,FileMode.Create);		
		BinaryWriter bw=new BinaryWriter(file);
		bw.Write(bytes);
		file.Close();
		
		
#endif		
	}
	
	

#if UNITY_EDITOR
	public static Texture2D importTexture (string assetPath, TextureImporterFormat textureFormat=TextureImporterFormat.RGBA32, int maxTextureSize=1024)
	{
		Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(assetPath,typeof(Texture2D));
		if (texture==null)
			return null;
		TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
		textureImporter.textureFormat=textureFormat;
		textureImporter.textureType=TextureImporterType.Advanced;
		textureImporter.mipmapEnabled=false;
		textureImporter.wrapMode=TextureWrapMode.Clamp;
		textureImporter.filterMode=FilterMode.Point;
		textureImporter.npotScale=TextureImporterNPOTScale.None;
		textureImporter.maxTextureSize=maxTextureSize;
		textureImporter.isReadable=true;
		AssetDatabase.ImportAsset(assetPath);
		return (Texture2D) AssetDatabase.LoadAssetAtPath(assetPath, typeof (Texture2D));
	}
#endif
	
	static float t3;
	public static Color32[] mergetTextureAbovePixelArray(Texture2D topTexture, Color32[] backColors){
		Color32[] textureColors = topTexture.GetPixels32();
		if (textureColors.Length != backColors.Length )
			Debug.LogError("texture should have the same size as input color array");
		for (int i = 0; i < backColors.Length; i++) {
			if (backColors[i].a>0){
				t3 = (float)textureColors[i].a/255.0f;
				textureColors[i].r = (byte) ((double) backColors[i].r + (double) ((int) textureColors[i].r - (int) backColors[i].r) * t3);
				textureColors[i].g = (byte) ((double) backColors[i].g + (double) ((int) textureColors[i].g - (int) backColors[i].g) * t3);
				textureColors[i].b = (byte) ((double) backColors[i].b + (double) ((int) textureColors[i].b - (int) backColors[i].b) * t3);			
				textureColors[i].a = 255;	
			}
		}
		return textureColors;
	}




	static int _leftX,_rightX,_downY,_topY,_srcLeftX,_srcDownY,_srcY,_destY,_srcX,_x,_y;
	
	public static void applyTextureToAnotherTexture (ref TextureColorArray source, ref TextureColorArray destination, IntVector2 centerInDestination) {
		_leftX  = centerInDestination.x - source.width / 2;
		_rightX = _leftX + source.width;
		_downY  = centerInDestination.y - source.height / 2;
		_topY   = _downY + source.height;
		
		_srcLeftX = _leftX < 0 ? -_leftX : 0;
		_srcDownY = _downY < 0 ? -_downY : 0;
		
		_leftX  = _leftX  <  0                  ? 0                      : _leftX ;
		_rightX = _rightX >= destination.width  ? destination.width    : _rightX;
		_downY  = _downY  <  0                  ? 0                      : _downY ;
		_topY   = _topY   >= destination.height ? destination.height  : _topY  ;
		
		
		_srcY = _srcDownY * source.width;
		_destY = _downY * destination.width;
		for (_y = _downY; _y < _topY; _y++) {						
			_srcX = _srcLeftX;			
			for (int _x = _leftX; _x < _rightX; _x++) {						
				
				if (source.Colors [_srcY  + _srcX].a > destination.Colors [_destY + _x].a)
					destination.Colors [_destY + _x] = source.Colors [_srcY  + _srcX];
				_srcX ++;
			}
			_srcY  += source.width;
			_destY += destination.width;
		}
		
	}
	
	public static void applyTextureToAnotherTextureWithPixelRect (ref TextureColorArray source, ref TextureColorArray destination, IntVector2 centerInDestination) {
		Rect pixelRect = source.pixelRect;
		
		int leftX = (int)(centerInDestination.x - pixelRect.width / 2);
		int downY = (int)(centerInDestination.y - pixelRect.height / 2);
		
		
		int srcY = (int)pixelRect.y - 1;
		for (int y = downY; y < (downY + pixelRect.height); y++) {			
			srcY++;
			int srcX = (int)pixelRect.x - 1;
			if (y < 0 || y >= destination.height)
				continue;
			for (int x = leftX; x < (leftX + pixelRect.width); x++) {
				srcX ++;
				if (x < 0 || x >= destination.width)
					continue;
				destination.Colors [y * destination.width + x] = source.Colors [srcY * source.width + srcX];
			}			
		}		
	}

	public static void cutoutTextureColorsByMask (ref Color32[] textureColors, Color32[] maskColorsArray, Color32 patternMaskColor) {
		for (int i = 0; i < maskColorsArray.Length; i++) {
			if (maskColorsArray[i].r != patternMaskColor.r
			    ||maskColorsArray[i].g != patternMaskColor.g
			    ||maskColorsArray[i].b != patternMaskColor.b)			
				textureColors [i] = EMPTY_COLOR;
		}
		
	}
	
	public static Color32[] generateTextureColorsByPatternAndPoints (int textureWidth, int textureHeight, Texture2D pattern, List<IntVector2> points) {		
		Color32[] newColors = new Color32[textureWidth * textureHeight];
			
		TextureColorArray src = new TextureColorArray (pattern.width, pattern.height, pattern.GetPixels32 ());
		TextureColorArray dst = new TextureColorArray (textureWidth, textureHeight, newColors);
			
		foreach (IntVector2 point in points) {
			TextureUtil.applyTextureToAnotherTexture (ref src, ref dst, point);
		}
		return newColors;
	}


	public static Color32[] generateTextureColorsByPatternAndPoint (int textureWidth, int textureHeight, Texture2D pattern, IntVector2 point) {		
		Color32[] newColors = new Color32[textureWidth * textureHeight];		
		TextureColorArray src = new TextureColorArray (pattern.width, pattern.height, pattern.GetPixels32 ());
		TextureColorArray dst = new TextureColorArray (textureWidth, textureHeight, newColors);	
		TextureUtil.applyTextureToAnotherTexture (ref src, ref dst, point);
		return newColors;
	}


	#region FloodFill impl
	
	private struct PointsRegion {
		public int direction;


		public int xLeft;
		public int xRight;
		public int y;

		public override string ToString(){
			return ("(direction="+direction+",x["+xLeft+"->"+xRight+"] y="+y+")");
		}
	}

	static int originalXLeft, originalXRigth;
	static bool[,] resultBoolRegion;

	public static bool[,] floodFillLineGetRegion (IntVector2 point, Color32[] colors, bool[] persistentLayer, int width, int height) {
		//go to left and to the right
		// if down pixel vector has border get righ pixel to bottomQueue
		// if upper pixel has unvisited node



		if (resultBoolRegion == null || resultBoolRegion.GetLongLength(0) != width || resultBoolRegion.GetLength(1) != height){
			resultBoolRegion = new bool[width, height];
		} else {
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					resultBoolRegion[i,j] = false;
				}
			}
		}

		Color32 seedColor = colors[point.y * width + point.x];
		PointsRegion initial = new PointsRegion ();
		initial.xLeft = point.x;
		initial.xRight = point.x;
		initial.y = point.y;
		initial.direction = 1;

		Queue queue = new Queue ();
		queue.Enqueue (initial);
		
		scanLeftPixelsForBound (colors,ref seedColor, persistentLayer, ref initial, ref width);
		scanRightPixelsForBound (width, colors,ref seedColor, persistentLayer, ref initial, ref width); 
		
		scanUpBelowPixelsRegionBackward (ref initial.xLeft, ref initial.xRight, ref initial.y, ref height, ref initial,
		                         ref colors, ref persistentLayer, ref seedColor, ref queue, resultBoolRegion, ref width);
		
		
		while (queue.Count>0) {                        
			PointsRegion pointsRegion = (PointsRegion)queue.Dequeue ();
			
			if (isPointsRegionVisited (pointsRegion, resultBoolRegion)) 
				continue;                       
			
			
			originalXLeft = pointsRegion.xLeft - 1;			
			scanLeftPixelsForBound (colors, ref seedColor, persistentLayer, ref pointsRegion, ref width);
			if (originalXLeft > pointsRegion.xLeft)
				scanUpBelowPixelsRegionBackward (ref pointsRegion.xLeft, ref originalXLeft, ref pointsRegion.y, ref height, ref pointsRegion, 
				                         ref colors, ref persistentLayer, ref seedColor, ref queue, resultBoolRegion, ref width);                 
			
			originalXRigth = pointsRegion.xRight+1;                  
			scanRightPixelsForBound (width, colors,ref seedColor, persistentLayer, ref pointsRegion, ref width);
			if (originalXRigth < pointsRegion.xRight)			
				scanUpBelowPixelsRegionBackward (ref originalXRigth, ref pointsRegion.xRight, ref pointsRegion.y, ref height, ref pointsRegion, 
				                         ref colors, ref persistentLayer, ref seedColor, ref queue, resultBoolRegion, ref width);
			
			for (int xx=pointsRegion.xLeft; xx<=pointsRegion.xRight; xx++) {							
				resultBoolRegion [xx, pointsRegion.y] = true;					
			}
			
			// 2. get DownPixel  -this is not exactly down pixel (it depends of direction) 
			scanUpBelowPixelsRegionForward (ref pointsRegion.xLeft, ref pointsRegion.xRight, ref pointsRegion.y, ref height, ref pointsRegion, 
			                         ref colors, ref persistentLayer, ref seedColor, ref queue, resultBoolRegion, ref width);   					
		}
		
		
		return resultBoolRegion;
	}

	static void scanLeftPixelsForBound (Color32[] colors,ref Color32 seedColor, bool[] persistentLayer, ref PointsRegion pointsRegion, ref int width)
	{
		for (int xx=pointsRegion.xLeft; xx>=0; xx--) {
			if (isPixelSeed (ref xx, ref pointsRegion.y,ref seedColor, ref colors, ref persistentLayer, ref width)) {
				pointsRegion.xLeft = xx;
			} else {
				break;  
			}
		                         
		}
	}

	static void scanRightPixelsForBound (int workspaceWidth, Color32[] colors,ref Color32 seedColor, bool[] persistentLayer, ref PointsRegion pointsRegion, ref int width)
	{
		for (int xx=pointsRegion.xRight; xx<workspaceWidth; xx++) {	
			if (isPixelSeed (ref xx, ref pointsRegion.y,ref seedColor, ref colors, ref persistentLayer, ref width)) {
				pointsRegion.xRight = xx;
			} else {
				break;
			}								
		}
	}

	static bool isPointsRegionVisited (PointsRegion pointsRegion, bool[,] resultBoolRegion)
	{
		return resultBoolRegion [pointsRegion.xLeft, pointsRegion.y];
	}
        
	//this function scan upper or below pixels diapason, and if it find new seedpixel regions add it to queue
	static PointsRegion __newPointsRegion;
	static int __yy,__xx, i31;
	static bool __prevPixelSeed;
	private static void scanUpBelowPixelsRegionForward (ref int xLeft, ref int xRight, ref int baseY, ref int maxY, ref PointsRegion pointsRegion, 
							ref Color32[] colors, ref bool[] persistentBorder, ref Color32 seedColor, ref Queue queue, bool[,] resultRegion, ref int width) {

		__newPointsRegion = new PointsRegion ();

		__yy = baseY + pointsRegion.direction;         
		if (__yy >= 0 && __yy < maxY) {
			__prevPixelSeed = false;   			
			for (__xx = xLeft; __xx<=xRight; __xx++) {
				i31 = __yy *width + __xx;
				if (resultRegion [__xx, __yy] != true
				    && (!persistentBorder [i31] //is pixel seed start
				    	&& ((colors [i31].a < 255) 
				    	     || (   seedColor.r == colors[i31].r 
				    		 && seedColor.g == colors[i31].g 
				    		 && seedColor.b == colors[i31].b 
				    		 && seedColor.a == colors[i31].a))) //is pixel seed end

				    ) {
					if (!__prevPixelSeed) {            
						__newPointsRegion.direction = pointsRegion.direction         ;
						__newPointsRegion.y         = __yy                ;                                                       
						__newPointsRegion.xLeft     = __xx                ;                                                 
						__prevPixelSeed             = true              ;						
					}                                               
				} else {
					if (__prevPixelSeed) {             
						__newPointsRegion.xRight  = __xx-1;                                        
						__prevPixelSeed = false;                                                    
						queue.Enqueue (__newPointsRegion);
					}                                       
				}                                                       
			}     
			if(__prevPixelSeed){
				__newPointsRegion.xRight = xRight;
				queue.Enqueue(__newPointsRegion);
			}
		}
	}

	static int i32;
	private static void scanUpBelowPixelsRegionBackward (ref int xLeft, ref int xRight, ref int baseY, ref int maxY, ref PointsRegion pointsRegion, 
	                                                    ref Color32[] colors, ref bool[] persistentBorder, ref Color32 seedColor, ref Queue queue, bool[,] resultRegion, ref int width) {
		
		__newPointsRegion = new PointsRegion ();
		
		__yy = baseY - pointsRegion.direction;         
		if (__yy >= 0 && __yy < maxY) {
			__prevPixelSeed = false;   			
			for (__xx = xLeft; __xx<=xRight; __xx++) {   
				i32 = __yy * width + __xx;
				if (resultRegion [__xx, __yy] != true
				    && (!persistentBorder [i32] // is pixel seed start
				         && ((colors [i32].a < 255) 
				              || (   seedColor.r == colors[i32].r 
				                  && seedColor.g == colors[i32].g 
				                  && seedColor.b == colors[i32].b 
				                  && seedColor.a == colors[i32].a))) //is pixel seed end

				    ) {
					if (!__prevPixelSeed) {            
						__newPointsRegion.direction = - pointsRegion.direction         ;
						__newPointsRegion.y         = __yy                ;                                                       
						__newPointsRegion.xLeft     = __xx                ;                                                 
						__prevPixelSeed             = true              ;						
					}                                               
				} else {
					if (__prevPixelSeed) {             
						__newPointsRegion.xRight  = __xx-1;                                        
						__prevPixelSeed = false;                                                    
						queue.Enqueue (__newPointsRegion);
					}                                       
				}                                                       
			}     
			if(__prevPixelSeed){
				__newPointsRegion.xRight = xRight;
				queue.Enqueue(__newPointsRegion);
			}
		}
	}


	static int i33;
	private static bool isPixelSeed (ref int x, ref int y, ref Color32 seedColor, ref Color32[] colors, ref bool[] persistentBorder, ref int width) {

		i33 = y*width + x;
		if (persistentBorder [i33])
			return false;			
			    
		if (colors [i33].a < 255)
			return true;

		if(seedColor.r != colors[i33].r
		   || seedColor.g != colors[i33].g 
		   || seedColor.b != colors[i33].b 
		   || seedColor.a != colors[i33].a )
			return false;
		return true;
	}
				
	private static byte byteAbs (byte a) {
		return (byte)((a < 0) ? -a : a);		
	}
        
	public static bool _colorEquals (Color a, Color b, float sensetivity) {
		if (Mathf.Abs (a [1] - b [1]) < sensetivity && 
                        Mathf.Abs (a [2] - b [2]) < sensetivity &&
                        Mathf.Abs (a [3] - b [3]) < sensetivity)
			return true;
		return false;
	}       
	#endregion



	static int sourceWidth,sourceHeight, sourceHalfWidth, sourceHalfHeight;
	static int leftX,rightX,downY,topY,srcLeftX,srcDownY, srcY,srcX, destY,x,y;
	static int currentY, currentX, destI, sourceI;
	static IntVector2 previousPoint;
	public static Color32[] generateTexturePath (CanvasTool canvasTool, Color32 activeColor, 
	                                      List<IntVector2> points, 
	                                      Color32[] destinationColors, int destinationWidth, int destinationHeight){
		sourceWidth = canvasTool.lineWidth;
		sourceHeight = canvasTool.lineHeight;
		sourceHalfWidth = sourceWidth / 2;
		sourceHalfHeight = sourceHeight / 2;
		previousPoint = new IntVector2(-10000,-10000);


		bool directPosition;

		for (int i = 0; i < points.Count; i++) {
			leftX  = points[i].x - sourceHalfWidth;
			downY  = points[i].y - sourceHalfHeight;
			//TODO Dictionary can be changed to delegates to optimise it here		
			PixelCache[] cache = getDirection(points[i], previousPoint, canvasTool, out directPosition);
			previousPoint = points[i];
			
			
			if (directPosition){
				sourceI = 0;
				for (int ii = 0; ii < sourceHeight; ii++) {
					for (int jj = 0; jj < sourceWidth; jj++) {

						currentY = (downY + ii);
						if (currentY > -1 && currentY < destinationHeight){
							currentX = (leftX + jj);
							if (currentX > -1 
							    && currentX < destinationWidth 
							    &&  canvasTool.colors[sourceI].a >= destinationColors[destI].a){
								destI = currentY * destinationWidth + currentX ;
								destinationColors[destI].r = activeColor.r;
								destinationColors[destI].g = activeColor.g;
								destinationColors[destI].b = activeColor.b;
								destinationColors[destI].a = canvasTool.colors[sourceI].a;
							}
						}
						sourceI++;
					}
				}
			} else {
				if (cache == null){
					continue;
				}

				for (int c = 0; c < cache.Length; c++) {
					currentY = (downY + cache[c].y);
					if (currentY > -1 && currentY < destinationHeight){
						currentX = (leftX + cache[c].x);
						if (currentX > -1 && currentX < destinationWidth){
							destI = currentY * destinationWidth + currentX;							
							destinationColors[destI].r = activeColor.r;
							destinationColors[destI].g = activeColor.g;
							destinationColors[destI].b = activeColor.b;
							destinationColors[destI].a = activeColor.a;
						}
					}
				}
			}
			
		}
		return destinationColors;
	}
	
	static int xxx,yyy;
	static PixelCache[] getDirection(IntVector2 currentPoint, IntVector2 previousPoint, CanvasTool canvasTool, out bool directPosition){
		directPosition = false;
		xxx = currentPoint.x - previousPoint.x;
		yyy = currentPoint.y - previousPoint.y;
		if ((xxx*xxx+yyy*yyy)>2){
			directPosition = true;
			return null;
		} else {
			return canvasTool.fullCache[yyy+1][xxx+1];
		}
	}
}
