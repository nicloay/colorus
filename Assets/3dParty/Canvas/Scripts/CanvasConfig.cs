using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class CanvasCameraConfig{
	public Color bgColor = Color.blue;
	public Rect screenPixelRect;
	public int cameraDepth;
	public float camMinSize = 20;
}

[Serializable]
public class CanvasConfig {
	public IntVector2 canvasSize;
	public Shader layersShader;
	public int bufferSize=3;
	public float scrollRatio = 100;

	public Shader radialFillShader;
	public Texture2D radialFillTexture;
}
