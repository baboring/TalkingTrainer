using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

// A dockable, context-sensitive search bar. Select a GameObject in the
// hierarchy to search its components and PlayMaker FSMs by name.
public class ComponentSearchBar : EditorWindow
{
	private Color foundColor = new Color(0.588f, 0.976f, 0.482f);
	private Color notFoundColor = new Color(1f, 0.68f, 0.68f);
	private Texture backButtonTexture;
	
	private string searchText = string.Empty;
	private GameObject prevSelection = null;
	private List<Component> matchesInChildren = new List<Component>();
	// This is where the back button takes us if pressed. Only set when we selected a child
	// via the "matches in children" subpanel.
	private GameObject parentToReturnTo = null;
	private int numMatchesFound = 0;
	
	private Vector2 childScrollerPosition = Vector2.zero;
	
	[MenuItem("Tools/Component Search Bar %&c")]
	public static ComponentSearchBar ShowSearchBar()
	{
		// Ensure the window can be docked
		var searchBar = (ComponentSearchBar)EditorWindow.GetWindow(typeof(ComponentSearchBar), false, "Component Search Bar", true);

		searchBar.backButtonTexture = AssetDatabase.LoadMainAssetAtPath("Assets/ComponentSearch/ComponentSearchBackButton.png") as Texture;
		
		return searchBar;
	}

	private void OnGUI()
	{
		var helpIcon = EditorGUIUtility.IconContent("_Help");

		if (GUI.Button(new Rect(0, 0, 20, 20), helpIcon, "Manual"))
		{
			Application.OpenURL("http://www.inkstarsoftware.com/component-search/manual.html");
		}

		if(Selection.activeGameObject)
		{
			if(searchText.Length == 0)
				GUI.backgroundColor = Color.white;
			else if(numMatchesFound > 0)
				GUI.backgroundColor = foundColor;
			else
				GUI.backgroundColor = notFoundColor;
		}
		else
		{
			// Nothing is selected. Be demure.
			GUI.backgroundColor = Color.white;
		}
		
		title = "Find";
		minSize = new Vector2(250f, 90f);

		// Draw the search bar
		searchText = GUI.TextField(new Rect(15, 0, Screen.width - 25, 15), searchText, GUI.skin.FindStyle("ToolbarSeachTextField"));

		// If we're in a child object and came here from a parent, show the back button
		if(parentToReturnTo != null)
		{
			var buttonStyle = GUI.skin.button;
			buttonStyle.padding = new RectOffset(0, 0, 0, 0);

			if (GUI.Button(new Rect(Screen.width - 23, 20, 20, 20), backButtonTexture, buttonStyle))
			{
				Selection.activeGameObject = parentToReturnTo;
				parentToReturnTo = null;
			}
		}
		
		DrawClearButton();
		
		DrawSearchResults();
	}

	private void DrawClearButton()
	{
		GUIStyle clearButtonStyle;
		
		if(searchText.Length == 0)
			clearButtonStyle = GUI.skin.FindStyle("ToolbarSeachCancelButtonEmpty");
		else
			clearButtonStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");
		
		clearButtonStyle.normal.textColor = Color.black;
		if (GUI.Button(new Rect(Screen.width - 15, 0, 15, 20), string.Empty, clearButtonStyle))
		{
			searchText = string.Empty;
		}
	}
	
	// On editor quit or window hide, show all components of selection
	private void OnDestroy()
	{
		ShowAllComponents(Selection.activeGameObject);		
	}
	
	private void OnSelectionChange()
	{
		Repaint();
	}
	
	private void DrawSearchResults()
	{
		if(prevSelection && prevSelection != Selection.activeGameObject)
		{
			// Unhide the components of the object we just deselected.
			ShowAllComponents(prevSelection);
		}
		
		numMatchesFound = 0;
		
		ShowSearchResults();
		
		FindMatchingChildren();
		ShowMatchingChildren();
		
		prevSelection = Selection.activeGameObject;
	}
	
	// Clears all hide flags on a GameObject, making all of its components
	// visible again.
	public static void ShowAllComponents(GameObject gameObj)
	{
		if(gameObj == null)
			return;
		
		foreach(var component in gameObj.GetComponents<Component>())
		{
			if(!component)
				continue;
				
			component.hideFlags = HideFlags.None;
		}
		
		foreach(var component in gameObj.GetComponentsInChildren<Component>())
		{
			if(!component)
				continue;
				
			component.hideFlags = HideFlags.None;
		}
	}
	
	// Filter the inspector to show only components that match the search.
	private void ShowSearchResults()
	{
		if(!Selection.activeGameObject)
			return;
			
		foreach(var component in Selection.activeGameObject.GetComponents<Component>())
		{
			if(!component)
				continue;
				
			if(DoesComponentMatch(component))
			{
				numMatchesFound++;
				component.hideFlags = HideFlags.None;
			}
			else
			{
				component.hideFlags = HideFlags.HideInInspector;
			}
		}
	}
	
	// True if the component's name matches our search string.
	private bool DoesComponentMatch(Component component)
	{
		string componentName = component.GetType().ToString();
		bool componentMatches = StringApproximatelyContains(componentName, searchText);
		
		if(StringApproximatelyContains(componentName, "PlayMakerFSM"))
		{
			// If this is a PlayMaker FSM, see if its name matches our search string.
			var fsmName = component.GetType().GetProperty("FsmName");

			if( fsmName != null )
			{
				componentMatches |= StringApproximatelyContains((string)fsmName.GetValue(component, null), searchText);
			}
		}
		
		return componentMatches;
	}
	
	private void FindMatchingChildren()
	{
		matchesInChildren.Clear();
		
		if(searchText == string.Empty)
			return;
		
		if(Selection.activeGameObject == null)
			return;
			
		// Check all components in the children of our selection.
		foreach(var childComp in Selection.activeGameObject.GetComponentsInChildren<Component>())
		{
			if(!childComp)
				continue;
				
			// GetComponentsInChildren returns the parent's components as well, so
			// ignore those
			if(childComp.gameObject == Selection.activeGameObject)
				continue;
				
			if(DoesComponentMatch(childComp))
			{
				numMatchesFound++;
				matchesInChildren.Add(childComp);
			}
		}
	}
	
	private void ShowMatchingChildren()
	{
		if(matchesInChildren.Count == 0)
			return;
			
		GUI.backgroundColor = Color.white;
		
		// Leave room for the search bar
		GUILayout.BeginVertical();
		GUILayout.Space(20f);
		GUILayout.EndVertical();

		if (matchesInChildren.Count > 0)
		{
			GUILayout.Label("Matches in children:", EditorStyles.boldLabel);
		}

		childScrollerPosition = EditorGUILayout.BeginScrollView(childScrollerPosition);
		
		foreach( var component in matchesInChildren)
		{
			EditorGUILayout.ObjectField(component.GetType().Name, component, typeof(Component), true);

			// If the user clicks on a matching child, select it. The matching components
			// will be shown automatically.
			if (Event.current.type == EventType.MouseUp && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
			{
				parentToReturnTo = Selection.activeGameObject;
				Selection.activeGameObject = component.gameObject;
			}
		}
		
		EditorGUILayout.EndScrollView();
	}
	
	// Search a string, ignoring case and spaces.
	private bool StringApproximatelyContains(string searchedString, string search)
	{
		if( searchedString == null || search == null )
			return false;

		if(search == string.Empty)
		{
			// No search entered? Match everything.
			return true;
		}

		searchedString = searchedString.Replace(" ", string.Empty).ToUpper();
		search = search.Replace(" ", string.Empty).ToUpper();
		
		return searchedString.Contains(search);
	}
}

public class ComponentSearchPostProcessor : UnityEditor.AssetModificationProcessor
{
	// This is called before a save. Show any components that were hidden
	// by the search bar to ensure that the hideFlags we set are never
	// serialized.
	public static string[] OnWillSaveAssets(string[] paths)
	{
		if(Selection.activeGameObject)
		{
			ComponentSearchBar.ShowAllComponents( Selection.activeGameObject );
		}
		
		return paths;
	}
}