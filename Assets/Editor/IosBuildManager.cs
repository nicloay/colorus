using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text.RegularExpressions;
using System.IO;



[Serializable]
public class IosBuildConfig{
	public string description;
	public string sceneName;
	public int sceneId;
	public SystemLanguage language = SystemLanguage.Russian;
	public GUIStampList stampList;
	public Albums albums;
	public string buildDestination;
	public string bundleId;
	
	static char SEPARATOR= '∆';
	
	public string toString(){
		return sceneName+ SEPARATOR +sceneId
						+ SEPARATOR +language.ToString()
						+ SEPARATOR +AssetDatabase.GetAssetPath(stampList)
						+ SEPARATOR +AssetDatabase.GetAssetPath(albums)
						+ SEPARATOR + buildDestination
						+ SEPARATOR + description
						+ SEPARATOR + bundleId;
	}
	
	public static IosBuildConfig fromString(string str, string[] sceneNames){
		IosBuildConfig result = new IosBuildConfig();
		string[] props = str.Split(SEPARATOR);
		if (props.Length != 8){
			Debug.LogError("wrong config numbers for conf part ["+str+"]   length="+props.Length);
			return result;
		}
		int sceneId = int.Parse(props[1]);
		if (sceneId > 0 && sceneId < sceneNames.Length){
			result.sceneId = sceneId;
			result.sceneName = sceneNames[result.sceneId];
		}
		result.language = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), props[2]);
		result.stampList = (GUIStampList)AssetDatabase.LoadAssetAtPath(props[3], typeof(GUIStampList));
		result.albums = (Albums)AssetDatabase.LoadAssetAtPath(props[4], typeof(Albums));
		result.buildDestination = props[5];
		result.description = props[6];
		if (props.Length == 8)
			result.bundleId = props[7];
		return result;
	}
}

public class IosBuildManager : EditorWindow {
	const char SEPARATOR = 'Ø';
	const string EDITOR_PREF_PATH="nicloay.razukrashka.builds";
	
	[MenuItem("Razukrashka/ios_build")]
	static void menuInit(){
		IosBuildManager manager = (IosBuildManager) EditorWindow.GetWindow(typeof(IosBuildManager));
		manager.init();
	}
	
	List<IosBuildConfig> configs;
	Queue<IosBuildConfig> removeQueue;
	Queue<IosBuildConfig> buildQueue;
	public void init(){
		configs = new List<IosBuildConfig>();
		removeQueue = new Queue<IosBuildConfig>();
		buildQueue = new Queue<IosBuildConfig>();
		refreshSceneNames();
		readConfig();
	}
	
	string[] sceneNames;
	void refreshSceneNames(){
		List<string> temp = new List<string>();
		foreach (UnityEditor.EditorBuildSettingsScene sceneSettings in UnityEditor.EditorBuildSettings.scenes)
		{
			
			temp.Add(sceneSettings.path);
		}
		sceneNames = temp.ToArray();
	}
	
	
	void OnGUI(){
		if (configs == null){
			EditorGUILayout.LabelField("no configs yiet");
			return;
		}

		foreach(IosBuildConfig config in configs)
			renderConfig(config);
		removeQuedConfigs();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("add new config"))
			addNewConfig();
		
		if (GUILayout.Button("refresh scene List"))
			refreshSceneNames();
		
		if (GUILayout.Button("save to Assets"))
			saveToAssets();
		
		if (GUILayout.Button("read from Assets"))
			readFromAssets();
		
		GUILayout.EndHorizontal();
	}
	
	void saveToAssets(){
		string path = EditorUtility.SaveFilePanelInProject("select file to save", "buildConfigs.asset","asset","yarrr!");
		path = path.Replace(Application.dataPath+"/", "" );
		IosBuildList buildList = ScriptableObject.CreateInstance<IosBuildList>();
		buildList.configs = configs;
		AssetDatabase.CreateAsset(buildList, path);
		AssetDatabase.SaveAssets();
	}
	
	void readFromAssets(){
		string path = EditorUtility.OpenFilePanel("select asset", Application.dataPath,"asset");
		if (path.Contains(Application.dataPath)){
			path = path.Replace(Application.dataPath+"/", "Assets/" );
			IosBuildList buildList = (IosBuildList) AssetDatabase.LoadAssetAtPath(path,typeof(IosBuildList));
			if (buildList != null)
				configs = buildList.configs;
			else
				Debug.LogError("some problems in asset loading, its null");
		} else {
			Debug.LogError("file must be within project folder");
		}
	}
	
	void Update(){
		if (configs == null 
		    || removeQueue == null 
		    || buildQueue == null 
		    || sceneNames == null)
			init ();
		
		if (buildQueue != null &&  buildQueue.Count > 0)
			doBuild(buildQueue.Dequeue());
	}
	
	void addNewConfig(){
		configs.Add(new IosBuildConfig());
		saveConfig();
	}
	
	
	void readConfig(){
		configs = new List<IosBuildConfig>();
		string[] props = EditorPrefs.GetString(EDITOR_PREF_PATH).Split(SEPARATOR);
		foreach(string str in props)
			if (!string.IsNullOrEmpty(str))
				configs.Add(IosBuildConfig.fromString(str, sceneNames));
	}
	
	void saveConfig(){
		string str="";
		foreach(IosBuildConfig config in configs)
			str+= SEPARATOR+ config.toString() ;
		str = str.Substring(1,str.Length - 1);
		EditorPrefs.SetString(EDITOR_PREF_PATH, str);
	}
	
	
	
	void removeQuedConfigs ()
	{
		while(removeQueue !=null &&removeQueue.Count > 0)
			configs.Remove(removeQueue.Dequeue());
	}
	
	
	void renderConfig(IosBuildConfig config){
		EditorGUI.BeginChangeCheck();
		
		GUILayout.BeginVertical(GUI.skin.box);
		GUILayout.BeginHorizontal();
		config.description = EditorGUILayout.TextField(config.description, EditorStyles.boldLabel);
		GUILayout.FlexibleSpace();
		config.bundleId = EditorGUILayout.TextField(config.bundleId, EditorStyles.largeLabel);

		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical(GUILayout.MinWidth(150));
		config.sceneId   = EditorGUILayout.Popup(config.sceneId, sceneNames);
		config.language  = (SystemLanguage) EditorGUILayout.EnumPopup(config.language);
		GUILayout.EndVertical();
		GUILayout.BeginVertical(GUILayout.MinWidth(150));
		config.stampList = (GUIStampList)   EditorGUILayout.ObjectField(config.stampList, typeof( GUIStampList), false);
		config.albums    = (Albums)         EditorGUILayout.ObjectField(config.albums, typeof(Albums), false);
		GUILayout.EndVertical();
		GUILayout.BeginVertical();
		string destinationFolder = String.IsNullOrEmpty( config.buildDestination ) ? "select destination folder" : config.buildDestination;
		if (GUILayout.Button(new GUIContent(destinationFolder,destinationFolder) , EditorStyles.textField,GUILayout.MinWidth(150)))
			config.buildDestination = EditorUtility.SaveFolderPanel("select build destination", config.buildDestination, "");
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("set active"))
			setActive(config);
		
		if (GUILayout.Button("build"))
			buildQueue.Enqueue(config);
		if (GUILayout.Button("remove"))
			removeQueue.Enqueue(config);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		if (EditorGUI.EndChangeCheck())
			saveConfig();
	}
	
	void setActive(IosBuildConfig config){
		if (config.albums != null && config.stampList != null){
			PropertiesSingleton.instance.guiStampList = config.stampList;
			PropertiesSingleton.instance.albums = config.albums;
			PropertiesSingleton.instance.language = config.language;
		} else {
			Debug.LogError("albums or stamplist is null");
		}
	}
	
	void doBuild(IosBuildConfig config){
		preprocess(config);
		MethodInfo mi = typeof(BuildPipeline).GetMethod("BuildPlayerInternalNoCheck", BindingFlags.NonPublic | BindingFlags.Static);//new Type[]{typeof(string[]), typeof(string),typeof(BuildTarget),typeof(BuildOptions),typeof(bool) 
		uint crc=0;

		mi.Invoke(null, new object[]{
			new string[]{sceneNames[config.sceneId]},
			config.buildDestination,
			BuildTarget.iPhone,
			BuildOptions.None,
			false,
			crc
		});

		postProcess();
	}

	class MovedResource{
		public string from;
		public string to;
		public MovedResource (string from, string to)
		{
			this.from = from;
			this.to = to;
		}	
	}

	const string PICTURE_RESOURCE_DIR = "Textures/Pictures";
	const string STAMP_RESOURCE_DIR = "Textures/stamps";
	const string MOVE_RESOURCE_POSTFIX = "_disabled_";
	List<MovedResource> movedDirResources;
	List<MovedResource> movedFileResources;
	SystemLanguage oldLanguage;
	GUIStampList   oldStampList;
	Albums         oldAlbums;
	string         oldBundleId;
	string tempDir;
	void preprocess(IosBuildConfig config){
		oldLanguage = PropertiesSingleton.instance.language;
		PropertiesSingleton.instance.language = config.language;

		oldStampList = PropertiesSingleton.instance.guiStampList;
		PropertiesSingleton.instance.guiStampList = config.stampList;

		oldAlbums  = PropertiesSingleton.instance.albums;
		PropertiesSingleton.instance.albums = config.albums;

		oldBundleId = PlayerSettings.bundleIdentifier;
		PlayerSettings.bundleIdentifier = config.bundleId;
		tempDir = Directory.GetParent( Application.dataPath)+"/temp"+EditorApplication.timeSinceStartup.GetHashCode();
		Directory.CreateDirectory(tempDir);

		movedDirResources = new List<MovedResource>();
		movedFileResources = new List<MovedResource>();

		List<string> pictureNames = new List<string>();
		foreach(Album album in config.albums.album){
			if (album.sheetList!=null)
				foreach(SheetObject sheet in album.sheetList.sheetList)
					pictureNames.Add(sheet.persistentBorderLayerPath);
		}
		foreach(string resourceDir in Directory.GetDirectories(Application.dataPath+"/"+ PICTURE_RESOURCE_DIR,"Resources", SearchOption.AllDirectories)){
			string[] filePaths = Directory.GetFiles(resourceDir,"*.border.*");
			if (filePaths.Length > 0){
				string borderName = Path.GetFileNameWithoutExtension(filePaths[0]);
				if (!pictureNames.Contains(borderName)){
					movedDirResources.Add(new MovedResource( resourceDir        , tempDir + "/" + resourceDir.GetHashCode()));
					movedFileResources.Add(new MovedResource( resourceDir+".meta", tempDir + "/" + resourceDir.GetHashCode()+"meta"));
				}
			}
		}



		List<string> stamps = new List<string>();
		foreach (GUIStamp stamp in config.stampList.stampList)
			stamps.Add(stamp.stampPath);

		foreach(string filePath in Directory.GetFiles(Application.dataPath + "/" + STAMP_RESOURCE_DIR, "*.icon")){
			if (filePath.EndsWith("meta"))
				continue;
			string fileName =Path.GetFileNameWithoutExtension(filePath);
			fileName = fileName.Replace(".icon","");
			if (!stamps.Contains(fileName)){
				movedFileResources.Add(new MovedResource(filePath        , tempDir + "/" + filePath.GetHashCode()+  ".icon"));
				movedFileResources.Add(new MovedResource(filePath+".meta", tempDir + "/" + filePath.GetHashCode()+  ".icon.meta"));
			}
		}

		foreach(string filePath in Directory.GetFiles(Application.dataPath + "/" + STAMP_RESOURCE_DIR+"/Resources")){
			if (filePath.EndsWith("meta"))
				continue;
			string fileName =Path.GetFileNameWithoutExtension(filePath);
			if (!stamps.Contains(fileName)){
				movedFileResources.Add(new MovedResource(filePath        , tempDir + "/" + filePath.GetHashCode()));
				movedFileResources.Add(new MovedResource(filePath+".meta", tempDir + "/" + filePath.GetHashCode()+".meta"));
			}
		}

		foreach(MovedResource mr in movedDirResources){
			Directory.Move(mr.from, mr.to);
		}

		foreach(MovedResource mr in movedFileResources)
			File.Move(mr.from, mr.to);

		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

		PropertiesSingleton.instance.language = config.language;
	}



	void postProcess(){
		PropertiesSingleton.instance.language = oldLanguage;

		PropertiesSingleton.instance.guiStampList = oldStampList;
				  
		PropertiesSingleton.instance.albums = oldAlbums;

		PlayerSettings.bundleIdentifier = oldBundleId;

		foreach(MovedResource mr in movedDirResources)
			Directory.Move(mr.to, mr.from);

		foreach(MovedResource mr in movedFileResources)
			File.Move(mr.to, mr.from);

		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
		Directory.Delete(tempDir);
	}
}