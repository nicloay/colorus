using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;


namespace localisation{
	public class InspectorTooltipWindow : EditorWindow  {
		string key;
		List<ItemStruct> list;
		public StringEditor pd;


		class ItemStruct{
			public SystemLanguage    lang;
			public LanguageFileEntry entry;
			public LanguageFile      langFile;
			public string            currentString;
		}


		public void setParameters(string key, Locale locale){
			this.key = key;
			list = new List<ItemStruct>();
			foreach (LanguageFile lf in  Locale.instance.languages){
				EditorGUILayout.LabelField(lf.language.ToString());
				ItemStruct str = new ItemStruct();
				str.currentString = "";
				str.langFile = lf;
				str.lang = lf.language;
				for (int i = 0; i < lf.entries.Count; i++) {
					if (lf.entries[i].key.Equals( key)){
						str.entry = lf.entries[i];
						str.currentString = lf.entries[i].value;
					}						
				}
				list.Add(str);
			}
			this.name = key;
		}
		string newS;
		void OnGUI(){
			if (list ==null)
				return;
			EditorGUILayout.LabelField(key);
			for (int i = 0; i < list.Count; i++) {
				ItemStruct str = list[i];

				EditorGUILayout.LabelField(str.lang.ToString());
				EditorGUI.BeginChangeCheck();
				newS = EditorGUILayout.TextArea(str.currentString);
				if (EditorGUI.EndChangeCheck()){
					str.currentString = newS;
					if (str.entry==null){
						list[i].entry = new LanguageFileEntry(key,newS);
						str.langFile.entries.Add(str.entry);
					} else {
						str.entry.value = newS;
					}
					EditorUtility.SetDirty(str.langFile);
				}

				EditorGUILayout.Space();

			}
		}

		void OnLostFocus(){
			pd.closeToolTip = true;
			Repaint();
		}
	}
}