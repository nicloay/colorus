using UnityEngine;
using System.Collections;

public class HelpViewWrapper : MonoBehaviour {

	public HelpView helpView;
	public GUISkin skin;

	void Awake() {
		helpView.recalculatePosition(skin);
	}

	void OnGUI () {
		GUI.skin = helpView.config.skin;
		helpView.OnGUI();
	}
	

	void Update () {
		if (Time.frameCount %50 == 0)
			helpView.recalculatePosition(skin);
	}
}
