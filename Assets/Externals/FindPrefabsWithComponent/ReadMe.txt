----------------------------
Find Prefabs With Component
----------------------------

Created by jake@beakerandjake.com 2015.
Tested in Unity 5.

----------------------------
Description. 
----------------------------

A simple Editor extension which allows you to easily find all prefabs which have a component attached. 

This extension will automatically select all matching, enabling you to quickly multi-edit them, or drag them into the scene. 

----------------------------
Usage
----------------------------

1. Extract the plugin to a location in your project.

2. In the Editor's project view simply navigate to the desired script file, right clicking the file will bring up the context menu. 

3. Locate the option that says "Find References in Prefabs" (Depending on the other extensions you might have installed, the position of the option may vary)

4. Wait for the search. This could take anywhere from under a second to several minutes in a huge project. (NOTE: If you're in a gigantic project and get tired of waiting, you can cancel the search by clicking the "Cancel" option on the progress bar)

5. If prefabs were found they will be automatically selected for you and the project view should display one of them. 

6. Now you can multi-edit all prefabs via the inspector, or drag those suckers into the scene. 


----------------------------
Why is the option grayed out?
----------------------------

The "Find References in Prefabs option will only be usable if:

1. You have a script asset selected

AND

2. That script derives from MonoBehaviour. 


For example if you have an editor script that derives from "EditorWindow" you cannot search in prefabs because "EditorWindow" doesn't derive from MonoBehaivour and can't be attached to a prefab. 