using UnityEngine;
using System.Collections;

public class WorkSpaceData {
	public LayerController workingLayer;
	public LayerController[] visibleLayers;
	public bool[,] persistentLayer;
	public Color32[] cacheColors;
	public int width;
	public int height;
}
