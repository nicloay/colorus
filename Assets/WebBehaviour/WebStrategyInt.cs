using UnityEngine;
using System.Collections;
using System;

public interface WebStrategyInt{
	bool isReady();
	void onStart(string data);
	void onNewPictureOpen(SheetObject sheetObject);
	void onPictureSave(Texture2D texture, string pictureName);
}
