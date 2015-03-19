using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using localisation;
using System.Linq;

[Serializable]
public class HelpViewConfig{
	[NonSerialized]
	public GUISkin skin;
	public IntVector2 windowSize;
	public string windowCaption;
	public int mainPadding = 5;
	public int leftColumnWidth;
	public int rowHeight;
	public int rowVPadding;

	public int sectionCaptionHeight;
	
	[NonSerialized]
	public string sectionCaptionStyleString = "helpSectionCaptionStyle";
	public GUIStyle sectionCaptionStyle;

	[NonSerialized]
	public string sectionButtonStyleString = "sectionButton";
	public GUIStyle sectionButtonStyle;

	[NonSerialized]
	public Rect windowRect;
	
}

public enum HelpItemType{
	IMAGE,
	STYLED_BUTTON
}


[Serializable]
public class HelpItem{
	public Vector2 		contentSize;
	public RectOffset	textOffset;
	public string  		text;
	public int     		rowTake;
	public HelpItemType 	type;

	public Texture2D        texture;
	
	public string           styleName;
	GUIStyle _style;
	public GUIStyle         style{
		get{
			return _style;
		}
	}

	[NonSerialized]
	public string           textStyleName="helpItemText";
	GUIStyle _textStyle;
	GUIStyle textStyle  {
		get{
			return _textStyle;
		}
	}


	Rect _itemRect;
	public Rect itemRect{	
		get {
			return _itemRect;
		}
	}

	Rect _textRect;
	public Rect textRect{
		get {
			return _textRect;
		}
	}

	Rect _contentRect;
	public Rect contentRect{
		get {
			return _contentRect;
		}
	}

	public void OnGUI(){
		if (texture != null)
			GUI.Box(contentRect, texture, style);
		GUI.Label(textRect, text.Localized(), textStyle);

	}

	public void setProps(Rect itemRect, int padding, GUISkin skin){
		if (styleName == "")
			_style = GUIStyle.none;
		else
			_style = skin.FindStyle(styleName);
		if (_style == null)
			_style = GUIStyle.none;

		if (textStyleName=="")
			_textStyle = GUIStyle.none;
		else 
			_textStyle = skin.FindStyle(textStyleName);
		if (_textStyle == null)
			_textStyle = GUIStyle.none;

		_itemRect  = itemRect;





		int middleY =(int)( _itemRect.y + _itemRect.height / 2);
		if (contentSize.y < itemRect.height ){
			int padd =(int)( (itemRect.height - contentSize.y) / 2);
			_contentRect = new Rect (0, _itemRect.y + padding, contentSize.x, contentSize.y);
			_textRect = new Rect(_contentRect.width + padding, _itemRect.y, 0, _itemRect.height);
			_textRect.width = _itemRect.width - _textRect.x ;
		} else {
			int width =(int)((float)itemRect.height  / (float)contentSize.y * (float) contentSize.x);
			_contentRect = new Rect(0, _itemRect.y , width, itemRect.height);
			_textRect = new Rect(_contentRect.width + padding, _itemRect.y, itemRect.width - _contentRect.width - padding, itemRect.height);
		}
		_textRect.width  -= textOffset.horizontal;
		_textRect.height -= textOffset.vertical;
		_textRect.x += textOffset.left;
		_textRect.y += textOffset.top;
	}

}

[Serializable]
public class HelpSection{
	public string name;

	public List<HelpItem> items;

	public int totalRowTake{
		get{
			if (items== null||items.Count == 0 )
				return 0;
			int result = 1;
			for (int i = 0; i < items.Count; i++) {
				result+= items[i].rowTake;
			}
			return result;
		}
	}

	[NonSerialized]
	public int yPosition;
	[NonSerialized]
	public Rect boxRect;
	[NonSerialized]
	public Rect captionRect;
	[NonSerialized]
	public int startYPosition;

}


[Serializable]
public class HelpView : GUIPart {
	public HelpViewConfig config;
	public List<HelpSection> sections;


	GuiModalWindow window;
	Rect sectionRect;
	Rect scrollAreaRect;
	Rect scrollAreaViewRect;

	float contentWidth;

	int selectedSectionId;
	string[] _sectionNames;
	Vector2 scrollPosition;
	public string[] sectionNames{
		get{
			if (_sectionNames == null || _sectionNames.Length != sections.Count)
				_sectionNames = new string[sections.Count];
			for (int i = 0; i < sections.Count; i++) {
				_sectionNames[i]=sections[i].name.Localized();
			}
			return _sectionNames;
		}
	}


	#region implemented abstract members of GUIPart
	public override void OnGUI () {
		window.OnGUI(new GUIContent(config.windowCaption));
	}
	#endregion


	HelpItem item;
	int newSectionId;
	Vector2 newScrollPosition;
	public void doMyWindow(){
		newSectionId = GUI.SelectionGrid(sectionRect,selectedSectionId, sectionNames, 1, config.sectionButtonStyle );
		if (newSectionId != selectedSectionId)
			scrollPosition.y = sections[newSectionId].yPosition;
		selectedSectionId = newSectionId;
		newScrollPosition = GUI.BeginScrollView(scrollAreaRect, scrollPosition, scrollAreaViewRect, false,true);
		if (newScrollPosition.y != scrollPosition.y){
			selectedSectionId = sections.IndexOf( sections.Aggregate((x,y) => Mathf.Abs(x.yPosition - newScrollPosition.y)
			                                                          < Mathf.Abs(y.yPosition - newScrollPosition.y) ? x : y));
		}


		scrollPosition = newScrollPosition;

		for (int i = 0; i < sections.Count; i++) {
			GUI.BeginGroup(sections[i].boxRect);
			GUI.Label(sections[i].captionRect, sections[i].name.Localized(), config.sectionCaptionStyle);
			for (int j = 0; j < sections[i].items.Count; j++) {
				sections[i].items[j].OnGUI();


			}
			GUI.EndGroup();
		}
		GUI.EndScrollView();
	}


	
	public void recalculatePosition(GUISkin skin){
		config.skin = skin;
		config.sectionCaptionStyle = config.skin.FindStyle(config.sectionCaptionStyleString);
		config.sectionButtonStyle  = config.skin.FindStyle(config.sectionButtonStyleString);


		config.windowRect = new Rect((Screen.width - config.windowSize.x)/2,
		                             (Screen.height - config.windowSize.y)/2,
		                             config.windowSize.x,
		                             config.windowSize.y);
		window = new GuiModalWindow();
		window.setProperties(config.windowRect, new GUIContent( config.windowCaption),config.skin,doMyWindow,onWindowExit,onWindowExit);


		sectionRect = new Rect(0, 0, 
		                       config.leftColumnWidth, config.windowSize.y );

		GUIStyle verticalScrollBar = config.skin.FindStyle("verticalScrollbar");
		int scrollWidth =(int)( verticalScrollBar.fixedWidth + verticalScrollBar.margin.left + verticalScrollBar.margin.right);
		contentWidth = config.windowSize.x - config.mainPadding  - scrollWidth - config.leftColumnWidth;
		scrollAreaRect = new Rect(config.mainPadding + config.leftColumnWidth,
		                          0,
		                          config.windowSize.x - config.mainPadding - config.leftColumnWidth,
		                          config.windowSize.y);
		scrollAreaViewRect = new Rect(0,0,contentWidth, 2000);

		fillSectionRects();
	}


	void fillSectionRects(){
		int sectionY = 0;
		int totalSectionHeight;
		for (int i = 0; i < sections.Count; i++) {
			totalSectionHeight = config.sectionCaptionHeight 
				+ sections[i].totalRowTake * (config.rowHeight + config.rowVPadding) 
					- config.rowVPadding;
			sections[i].boxRect = new Rect(0, sectionY, contentWidth, totalSectionHeight);
			sections[i].captionRect = new Rect(0, 0, contentWidth, config.sectionCaptionHeight);
			sections[i].yPosition = sectionY;
			sectionY += (totalSectionHeight + config.rowVPadding);

			int itemY = config.sectionCaptionHeight + config.rowVPadding;
			for (int j = 0; j < sections[i].items.Count; j++) {
				Rect itemRect = new Rect(0, 
				                         itemY, 
				                         contentWidth, 
				                         sections[i].items[j].rowTake * (config.rowHeight +config.rowVPadding) -config.rowVPadding);
				sections[i].items[j].setProps(itemRect, config.rowVPadding, config.skin );
				itemY += (int)itemRect.height + config.rowVPadding;
			}

		}
		scrollAreaViewRect.height = sectionY - config.rowVPadding;
	}


	public void onWindowExit(){
	}
}

