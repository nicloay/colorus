using UnityEngine;
using System.Collections;

public class WindowSoundController : ControllerInterface {
	GameState previousGameState = GameState.IN_GAME;
	SoundPlayer soundPlayer;

	#region ControllerInterface implementation
	public void init () {
		soundPlayer = SoundPlayer.instance;
		WorkspaceEventManager.instance.onSoundButtonClick            += onSoundButtonClickListener;
		WorkspaceEventManager.instance.onExitFromSoundSettingsWindow += onExitFromSoundSettingsWindowListener;

		WorkspaceEventManager.instance.onSoundWindowMuteClick        += onSoundWindowMuteClickListener;
		WorkspaceEventManager.instance.onSoundWindowMusicMuteClick   += onSoundWindowMusicMuteClickListener;
		WorkspaceEventManager.instance.onSoundWindowSFXMuteClick     += onSoundWindowSFXMuteClickListener;
		WorkspaceEventManager.instance.onSoundWindowMusicLevelChange += onSoundWindowMusicLevelChangeListener;
		WorkspaceEventManager.instance.onSoundWindowSFXLevelChange   += onSoundWindowSFXLevelChangeListener;	
	}
	#endregion



	void onSoundButtonClickListener () {
		previousGameState = PropertiesSingleton.instance.gameState;
		PropertiesSingleton.instance.gameState = GameState.SOUND_SETTINGS;
	}

	void onExitFromSoundSettingsWindowListener () {
		PropertiesSingleton.instance.gameState = previousGameState;
	}

	void onSoundWindowMuteClickListener () {
		PropertiesSingleton.instance.soundProperties.mute = !PropertiesSingleton.instance.soundProperties.mute;	
		soundPlayer.updateMusicStatus();
	}

	void onSoundWindowMusicMuteClickListener () {
		if (PropertiesSingleton.instance.soundProperties.mute){
			PropertiesSingleton.instance.soundProperties.mute = false;
			PropertiesSingleton.instance.soundProperties.musicMute = false;
		} else {
			PropertiesSingleton.instance.soundProperties.musicMute = !PropertiesSingleton.instance.soundProperties.musicMute;
			if (!PropertiesSingleton.instance.soundProperties.musicMute && PropertiesSingleton.instance.soundProperties.mute)
				PropertiesSingleton.instance.soundProperties.mute = false;
		}
		soundPlayer.updateMusicStatus();
	}

	void onSoundWindowSFXMuteClickListener () {
		if (PropertiesSingleton.instance.soundProperties.mute){
			PropertiesSingleton.instance.soundProperties.mute = false;
			PropertiesSingleton.instance.soundProperties.sfxMute = false;
		} else {
			PropertiesSingleton.instance.soundProperties.sfxMute = !PropertiesSingleton.instance.soundProperties.sfxMute ;
			if (!PropertiesSingleton.instance.soundProperties.sfxMute && PropertiesSingleton.instance.soundProperties.mute)
				PropertiesSingleton.instance.soundProperties.mute = false;
		}
	}

	void onSoundWindowMusicLevelChangeListener (float level) {
		PropertiesSingleton.instance.soundProperties.musicLevel = level;
		if (level>0){
			PropertiesSingleton.instance.soundProperties.musicMute = false;
			PropertiesSingleton.instance.soundProperties.mute      = false;
		}
		soundPlayer.updateLevels();
	}

	void onSoundWindowSFXLevelChangeListener (float level) {
		PropertiesSingleton.instance.soundProperties.sfxLevel = level;
		if (level>0){
			PropertiesSingleton.instance.soundProperties.sfxMute   = false;
			PropertiesSingleton.instance.soundProperties.mute      = false;
		}
	}
}
