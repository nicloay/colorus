using UnityEngine;
using System.Collections;

public class WrongActionController : ControllerUpdateInterface {
	PropertiesSingleton props;

	public void init () {
		props = PropertiesSingleton.instance;
		WorkspaceEventManager.instance.onWrongAction += onWrongactionListener;
	}


	float endTime;

	void onWrongactionListener (string errString) {
		Debug.Log("some problem happened: "+errString);
		props.wrongActionShowFrame = true;
		endTime = Time.time + props.wrongActionShowFrameTimeout;
	}


	public void update () {
		if (props.wrongActionShowFrame){
			if (Time.time > endTime){
				props.wrongActionShowFrame = false;
			}
		}
	}


}
