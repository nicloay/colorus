using UnityEngine;
using System.Collections;

public class CanvasBuffer {

	public int undoCount{
		get{
			return undoCounter;
		}
	}

	public int redoCount{
		get{
			return redoCounter;
		}
	}


	Color32[][] buffer;

	int bufferSize;
	int currentId;
	int undoCounter;
	int redoCounter;
	public CanvasBuffer(IntVector2 canvasSize, int bufferSize){
		int itemSize = canvasSize.x * canvasSize.y;
		buffer = new Color32[bufferSize][];
		for (int i=0; i< bufferSize;i++){
			buffer[i] = new Color32[itemSize];
		}
		this.bufferSize = bufferSize;
		currentId = 0;
		resetUndoRedo();
	}


	public void resetUndoRedo(){
		undoCounter = 0;
		redoCounter = 0;
	}

	public Color32[] getArray(){
		Color32[] result = buffer[currentId];
		if (undoCounter < bufferSize)
			undoCounter++;
		redoCounter = 0;
		ColorUtil.clearPixels(result);
		increment(ref currentId);

		return result;
	}

	public Color32[] getForUndo(){
		if (undoCounter > 0){
			undoCounter--;
			redoCounter++;
			decrement(ref currentId);
			return buffer[currentId];
		} else {
			Debug.LogWarning("no undo arrays here");
			return null;
		}
	}
 
	public Color32[] getForRedo(){
		if (redoCounter > 0){
			undoCounter++;
			redoCounter--;
			Color32[] result = buffer[currentId];
			increment(ref currentId);
			return result;
		} else {
			Debug.Log("no redo arrays here");
			return null;
		}
	}

	void increment(ref int i){
		i++;
		i = i == bufferSize ? 0 : i;
	}

	void decrement(ref int i){
		i--;
		i = i== -1? bufferSize -1 : i;
	}


}