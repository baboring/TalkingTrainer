using UnityEditor;
using UnityEngine;

public class MenuItems {

	[MenuItem("Tool/Clear PlayerPrefs")]
    private static void NewMenuOption()
    {
        PlayerPrefs.DeleteAll();
    }

}


