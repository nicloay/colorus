using UnityEngine;
using System.Collections;

public class ToolLogicContext  {
	private ToolLogicStrategy strategy;
	
	public ToolLogicContext(ToolLogicStrategy strategy){
		this.strategy=strategy;		
	}
#if !UNITY_IPHONE
	public void onMouseDown(IntVector2 position){
		strategy.onMouseDown(position);	
	}
	
	public void onMouseOverWithButton(IntVector2 position, Vector3 globalPosition){
		strategy.onMouseOverWithButton(position, globalPosition);
	}
	
	
	public void onMouseOverWithButtonDone(IntVector2 position){
		strategy.onMouseOverWithButtonDone(position);		
	}

	
	public void onMouseEnter(){
		strategy.onMouseEnter();	
	}
	
	public void onMouseExit(){
		strategy.onMouseExit();	
	}
	public void onMouseOver(IntVector2 pixelCursorPosition, Vector3 globalCursorPosition){
		strategy.onMouseOver (pixelCursorPosition,globalCursorPosition);
	}

#else 
	public void onTouchStart(IntVector2 pixelPosition, Vector3 globalPosition){
		strategy.onTouchStart(pixelPosition, globalPosition);
	}

	public void ontTouchOver(IntVector2 pixelPosition, Vector3 globalPosition){
		strategy.onTouchOver(pixelPosition, globalPosition);
	}

	public void onTouchEnd(){
		strategy.onTouchEnd();
	}
#endif

}
