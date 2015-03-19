using UnityEngine;
using System.Collections;

public class ScrollviewAutocloser {
	float topPading;
	float step;

	public ScrollviewAutocloser(float topPading, float step){
		this.topPading = topPading;
		this.step = step;
	}


	bool mouseUp = false;
	bool scroll = false;
	float scrollCloseTime;
	bool waitForScroll = false;
	float scrollTimeout = 0.2f;

	Vector2 lastPosition;
	float ignoreMovementAreaPixels = 20.0f;

	public bool needAnimation(){
		return animate;
	}

	public void preprocess(Event e, ref Vector2 scrollPosition){
		handleSwipe(e, ref scrollPosition);
		mouseUp = (e.type == EventType.mouseUp);
		scroll = (e.type == EventType.scrollWheel);
	}

	bool mouseDrag = false;
	void handleSwipe(Event e, ref Vector2 scrollPosition){

		if (e.type == EventType.MouseDrag){
			if (!mouseDrag){
				if ( Mathf.Abs( e.delta.x) > ignoreMovementAreaPixels
				    || Mathf.Abs( e.delta.y) > ignoreMovementAreaPixels){
					GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
					mouseDrag = true;
				} else {
					return;
				}
			}
#if UNITY_IOS
			scrollPosition += e.delta;
#else
			scrollPosition -= e.delta;
#endif
			e.Use();
		} else if (mouseDrag && e.type == EventType.MouseUp){
			mouseDrag = false;
			GUIUtility.hotControl = 0;
			verticalAlignItems(ref scrollPosition);
		}	
	}

	public void postProcess(Event e, ref Vector2 scrollPosition){
		if (mouseUp && Event.current.type == EventType.Used)
			verticalAlignItems(ref scrollPosition);
		if (scroll && e.type == EventType.Used){
			scrollCloseTime = Time.time + scrollTimeout;
			waitForScroll = true;
		}
		if (waitForScroll && Time.time > scrollCloseTime){
			waitForScroll = false;
			verticalAlignItems(ref scrollPosition);
		}
		
		
		if (animate)
			doScrollingToExactPosition(ref scrollPosition);
	}


	bool animate = false;
	float durationBaseTime = 0.4f;
	float duration = 0.4f;
	float startTime;
	float startY;
	float targetY;
	float routeLength;
	void verticalAlignItems(ref Vector2 scrollPosition){
		float yPos = scrollPosition.y  - topPading;
		float ratioPosition = yPos / step;
		float number = Mathf.Floor(ratioPosition);
		float reminder = ratioPosition - number;
		if (reminder == 0)
			return;
		if (reminder > 0.5f){
			targetY = (number + 1) * step + topPading;
			duration = durationBaseTime * (1- reminder);
		} else {
			targetY = number * step + topPading;
			duration = durationBaseTime * reminder;
		}
		startTime = Time.time;
		startY = scrollPosition.y;
		animate = true;
		routeLength = targetY - startY;
	}
	
	void doScrollingToExactPosition(ref Vector2 scrollPosition){
		float y;
		float time = Time.time - startTime;
		if (time < duration)
			y = InterpolationUtil.moveInCirc(time, startY, duration, routeLength);
		else{
			y = targetY;
			animate = false;
		} 
		scrollPosition.y = y;
	}

}
