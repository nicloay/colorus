using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum SoundType{
	MUSIC,
	SFX
}

public enum SoundPlayerState{
	NORMAL,
	SHIFTING
}

[System.Serializable]
public class SoundPlayerConfigTool{
	public AudioClip ink;
	public AudioClip brush;
	public AudioClip bigBrush;
	public AudioClip bucket;
	public AudioClip pipette;
	public AudioClip roller;
}

[System.Serializable]
public class SoundPlayerConfig{
	public AudioClip click;
	public AudioClip openWindow;
	public AudioClip closeWindow;
	public AudioClip error;
	public AudioClip reset;
	public AudioClip savePicture;
	public AudioClip scrollWheel;
	public AudioClip volumeSample;
	public AudioClip screenShot;

	public AudioClip panelSlide; //depricated
	public SoundPlayerConfigTool tools;
}


public class SoundPlayer : MonoSingleton<SoundPlayer> {
	public SoundPlayerConfig config;
	SoundProps mainSoundProps;
	Queue<AudioSource> audioSourcePool;
	SoundPlayerState state = SoundPlayerState.NORMAL;

	AudioSource music1Source;
	AudioSource music2Source;
	AudioSource sfxSource;


	#region ControllerInterface implementation
	void Start () {
		sfxSource = getAudioSource(SoundType.SFX);
		music1Source = getAudioSource(SoundType.MUSIC);
		music2Source = getAudioSource(SoundType.MUSIC);
		mainSoundProps = PropertiesSingleton.instance.soundProperties;
		subscribeToEvents();
		updateLevels();
		playRandomSong();
		updateMusicStatus();

	}
	#endregion

	void Update(){
		if ((music1Source.time + PropertiesSingleton.instance.soundProperties.shiftMusicTimeInSecons > music1Source.clip.length)
		    &&  state == SoundPlayerState.NORMAL)
			StartCoroutine(shiftMusic());
	}


	bool doWeNeedToplayTheSameSong;
	IEnumerator shiftMusic(){
		state = SoundPlayerState.SHIFTING;
		doWeNeedToplayTheSameSong = Random.Range(0,100) < PropertiesSingleton.instance.soundProperties.musicCycleChanceInPercent;

		if (doWeNeedToplayTheSameSong){
			yield return StartCoroutine( playTheSameSong ());
		} else  {
			yield return StartCoroutine( StartDifferentSong());
		}

		state = SoundPlayerState.NORMAL;
	}

	IEnumerator StartDifferentSong(){
		music2Source.enabled = true;
		AudioClip clip;
		do {
			clip = PropertiesSingleton.instance.soundProperties.music[Random.Range(0,PropertiesSingleton.instance.soundProperties.music.Length)];
		} while (clip == music1Source.clip);
		music2Source.clip = clip;
		music2Source.volume = 0;
		music2Source.Play();
		while(music1Source.isPlaying)
		{
			float ratio = (music1Source.clip.length - music1Source.time)/ PropertiesSingleton.instance.soundProperties.shiftMusicTimeInSecons;
			music1Source.volume = PropertiesSingleton.instance.soundProperties.musicLevel * ratio;
			music2Source.volume = PropertiesSingleton.instance.soundProperties.musicLevel * (1-ratio);
			yield return null;
		}	

		music1Source.enabled = false;
		AudioSource tmp = music1Source;

		music1Source = music2Source;
		music2Source = tmp;
	}


	IEnumerator playTheSameSong () {
		music1Source.loop = true;
		Debug.Log ("changed loop");
		while (music1Source.time > 10.0f) {
			yield return new WaitForSeconds (1.0f);
			Debug.Log ("wait 1 second");
		}
		Debug.Log ("changed loop back");
		music1Source.loop = false;
	}

	public void updateLevels(){
		music1Source.volume = PropertiesSingleton.instance.soundProperties.musicLevel;
		music2Source.volume = PropertiesSingleton.instance.soundProperties.musicLevel;
		sfxSource.volume    = PropertiesSingleton.instance.soundProperties.sfxLevel;
	}

	public void updateMusicStatus(){
		if (PropertiesSingleton.instance.soundProperties.doWeeNeedToPlayMusic){
			if (isMusicPlaying())
				return;
			playRandomSong();
		} else {
			if (!isMusicPlaying())
				return;
			music1Source.Stop();
			music2Source.Stop();
		}

	}


	bool isMusicPlaying(){
		return ((music1Source != null && music1Source.isPlaying)
		        || (music2Source != null && music2Source.isPlaying));
	}

	bool isSfxPlaying(){
		return sfxSource.isPlaying;
	}


	bool isPlaying(List<AudioSource> sourceList){
		if (sourceList.Count > 0){
			for (int i = 0; i < sourceList.Count; i++) {
				if (sourceList[i].isPlaying)
					return true;
			}
			return false;
		} else {
			return false;
		}
	}


	void playRandomSong(){
		AudioClip clip = PropertiesSingleton.instance.soundProperties.music[Random.Range(0,PropertiesSingleton.instance.soundProperties.music.Length)];
		music2Source.enabled = false;
		music1Source.clip = clip;
		music1Source.Play();
	}


	public AudioSource getAudioSource(SoundType soundType){
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		if (soundType == SoundType.MUSIC){
			audioSource.volume = PropertiesSingleton.instance.soundProperties.musicLevel;
		} else if (soundType == SoundType.SFX){
			audioSource.volume = PropertiesSingleton.instance.soundProperties.sfxLevel;	
		}

		return audioSource;
	}

	public void playSound(AudioClip clip){
		if (!mainSoundProps.mute && !mainSoundProps.sfxMute && (sfxSource.enabled && sfxSource.volume>0))
			sfxSource.PlayOneShot(clip);
	}



	void onButtonClickListener(){
		playSound(config.click);
	}

	void onButtonClickListener(int i){
		onButtonClickListener();
	}

	void onWindowOpenListener(){
		playSound(config.openWindow);
	}

	void onToolButtonClickListener(ToolType toolType){
		switch(toolType){
		case ToolType.BRUSH:
			playSound(config.tools.brush);
			break;
		case ToolType.BRUSH_LARGE:
			playSound(config.tools.bigBrush);
			break;
		case ToolType.BUCKET:
			playSound(config.tools.bucket);
			break;
		case ToolType.HAND:
			playSound(config.click);
			break;
		case ToolType.INK:
			playSound(config.tools.ink);
			break;
		case ToolType.PIPETTE:
		case ToolType.STAMP:
			playSound(config.tools.pipette);
			break;
		case ToolType.ROLLER:
			playSound(config.tools.roller);
			break;
		default:
			Debug.LogWarning("hmm... i don't know what to play, unkonw tool here");
			break;
		}


	}

	void onColorPickerCloseListener(Color32 color){
		playSound(config.closeWindow);
	}

	void onStampChooseListener(int i){
		playSound(PropertiesSingleton.instance.guiStampList.stampList[i].sound);
	}

	void onExitFromWindowListener(){
		playSound(config.closeWindow);
	}

	void onPictureSelect(int pictureId){
		playSound(config.reset);
	}

	void onSelectAlbum(int albumId){
		playSound(config.click);
	}

	void onSFXChangeLevel(float level){
		updateLevels();
		playSound(config.volumeSample);
	}

	void onWrongActionListener(string errString){
		playSound(config.error);
	}

	void subscribeToEvents(){
		subscribeToSFXEvents();

		WorkspaceEventManager.instance.onSoundWindowSFXLevelChange   += onSFXChangeLevel;
	}


	void onSavePictureListener(){
		playSound(config.screenShot);
	}


	void subscribeToSFXEvents(){
		WorkspaceEventManager.instance.onWrongAction                 += onWrongActionListener;
		
		WorkspaceEventManager.instance.onMenuNewPictureClick         += onWindowOpenListener;
		WorkspaceEventManager.instance.onSoundButtonClick            += onWindowOpenListener;
		WorkspaceEventManager.instance.onInfoButtonClick             += onWindowOpenListener;
		WorkspaceEventManager.instance.onPalleteOpenWindowClick      += onWindowOpenListener;
		WorkspaceEventManager.instance.onStampSelectButtonClick      += onWindowOpenListener;
		
		WorkspaceEventManager.instance.onToolButtonClick             += onToolButtonClickListener;
		
		WorkspaceEventManager.instance.onColorPickerClose            += onColorPickerCloseListener;
		WorkspaceEventManager.instance.onStampClickInWindow          += onStampChooseListener;
		
		
		WorkspaceEventManager.instance.onUndoClick                   += onButtonClickListener;
		WorkspaceEventManager.instance.onRedoClick                   += onButtonClickListener;
		WorkspaceEventManager.instance.onResetSheetClick             += onButtonClickListener;
		WorkspaceEventManager.instance.onSavePictureClick            += onSavePictureListener;
		WorkspaceEventManager.instance.onColorHistoryClick           += onButtonClickListener;
		WorkspaceEventManager.instance.onRandomActiveColorClick      += onButtonClickListener;
		WorkspaceEventManager.instance.onPredefinedColorClick        += onButtonClickListener;
		WorkspaceEventManager.instance.onDrawWithinRegionClick       += onButtonClickListener;
		
		WorkspaceEventManager.instance.onSoundWindowMusicMuteClick   += onButtonClickListener;
		WorkspaceEventManager.instance.onSoundWindowSFXMuteClick     += onButtonClickListener;
		WorkspaceEventManager.instance.onSoundWindowMuteClick        += onButtonClickListener;
		
		
		WorkspaceEventManager.instance.onExitFromStampChooserWindow  += onExitFromWindowListener;
		WorkspaceEventManager.instance.onExitFromPicChooserWindow    += onExitFromWindowListener;
		WorkspaceEventManager.instance.onExitFromInfoWindow          += onExitFromWindowListener;
		WorkspaceEventManager.instance.onExitFromSoundSettingsWindow += onExitFromWindowListener;
		
		WorkspaceEventManager.instance.onSelectPicture               += onPictureSelect;
		WorkspaceEventManager.instance.onSelectAlbum                 += onSelectAlbum;
	}
}
