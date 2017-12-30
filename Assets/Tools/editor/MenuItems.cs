using UnityEditor;
using UnityEngine;

public class MenuItems {

	[MenuItem("Tools/Clear PlayerPrefs")]
    private static void NewMenuOption()
    {
        PlayerPrefs.DeleteAll();
    }

}


