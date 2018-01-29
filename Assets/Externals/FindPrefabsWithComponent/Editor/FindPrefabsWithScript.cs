using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
/*
 * Created by Jake on 11-17-15 with the support of his cat Beaker. 
 * Tested in Unity 5.2.2f1
 * www.beakerandjake.com
 *  
 * */

/// <summary>
/// A custom right click menu item which searches all prefabs in the projects to see which have the selected script component attached. 
/// To use simply select a MonoBehaviour asset in the project view and right click it, choose the option "Find References in Prefabs".
/// If prefabs are found which have the selected MonoBeahviour component then they will be selected! 
/// </summary>
public class FindPrefabsWithScript
{
    #region Const Members
        /// <summary>
        /// The search pattern we use to find .prefab files. 
        /// </summary>
        private const string PREFAB_FILE_SEARCH_PATTERN = "t:Prefab";
        /// <summary>
        /// The format string for the searching progress bar title. 
        /// </summary>
        private const string PROGRESS_BAR_TITLE_FORMAT = "Searching for any prefabs with a {0} Component!";
        /// <summary>
        /// The format string for the searching progress bar message. 
        /// </summary>
        private const string PROGRESS_BAR_MESSAGE_FORMAT = "Searching prefab ({0}/{1})";
        /// <summary>
        /// The format string for when we get a very unlikely error case. 
        /// </summary>
        private const string ERROR_MESSAGE_FORMAT = "{0} encountered an unexptected error! Could not parse the System.Type from the script asset!";
        /// <summary>
        /// The format string we will use to display when we do not find any prefabs which have the specified component. 
        /// </summary>
        private const string NOT_FOUND_MESSAGE_FORMAT = "Did not find any prefabs which had a {0} Component attached!";
        /// <summary>
        /// The format string we will use when we have found results and want to display the count to the user. 
        /// </summary>
        private const string SUCCESS_MESSAGE_FORMAT = "Found {0} prefabs with an attached -{1}- Component!";
    #endregion Const Members

    #region Methods
        /// <summary>
        /// Our main function. When the user has a valid MonoBehaviour asset selected,
        /// they can right click, and choose the option specified by our MenuItem attribute. 
        /// That will call this function! 
        /// </summary>
        [MenuItem("Assets/Find References in Prefabs", false, 0)]
        static void Find()
        {
            //After testing it seems that Undo automatically records Selection changes. 
            //So I am not manually registering any Undo, since all this really does is modify the 
            //Selection.activeIds.

            //Get the System.Type of the Selection.activeObject. 
            var selectedType = GetSelectedType();

            //Sanity, shouldn't ever happen but lets be safe. 
            if (selectedType == null)
            {
                Debug.LogErrorFormat(ERROR_MESSAGE_FORMAT, typeof(FindPrefabsWithScript).Name);
                return;
            }

            //Get the path to every prefab in the project.
            var allMatches = GetPrefabsWhoHaveScript(selectedType).ToArray();

            //Sanity, bail if we got no results. 
            if (allMatches == null || allMatches.Length == 0)
            {
                DisplayNoResultsFoundMessage(selectedType);
                return;
            }

            //Display the results to the user. 
            HighlightResults(allMatches, selectedType);
        }

        /// <summary>
        /// Our validation function, will only allow the user to select the MenuItem 
        /// if a MonoScript is selected. 
        /// </summary>
        [MenuItem("Assets/Find References in Prefabs", true)]
        static bool IsValidScriptSelected()
        {
            //We only know how to find MonoBehaviours. 
            //Bail if the selected script did not derive from MonoBehaviour. 
            return IsMonoBehaviour(GetSelectedType());
        }

        /// <summary>
        /// Inspect the current activeObject Selection. Returns the Type name of the selected script. 
        /// </summary>
        /// <returns></returns>
        static System.Type GetSelectedType()
        {
            //Cast the current selection to a MonoScript (Unitys representation of a C#, JS or Boo file)
            var currentSelection = Selection.activeObject as MonoScript;

            //Our currentSelection shouldnt be null, our IsScriptSelected function should
            //prevent Unity from allowing us to run on a non-MonoScript object
            //But lets be safe. 
            if (currentSelection == null)
            {
                //If the cast failed then reutrn Empty so we know it failed. 
                return null;
            }

            //The selected object was indeed a script. Return the class name 
            return currentSelection.GetClass();
        }

        /// <summary>
        /// Just because a script file was selected, it doesn't mean that the script itself
        /// was a MonoBehaviour, it could be lots of other things. We can only search prefabs for 
        /// scripts which derive from MonoBehavior
        /// </summary>
        private static bool IsMonoBehaviour(System.Type selectedType)
        {
            //Sanity
            if (selectedType == null)
            {
                return false;
            }

            //Ensure that the selectedType derives from MonoBehaviour. 
            return typeof(MonoBehaviour).IsAssignableFrom(selectedType);
        }

        /// <summary>
        /// Log to the Console that we didn't find any results. 
        /// </summary>
        private static void DisplayNoResultsFoundMessage(System.Type selectedType)
        {
            Debug.LogFormat(NOT_FOUND_MESSAGE_FORMAT, selectedType.Name);
        }

        /// <summary>
        /// Search all prefabs in the project and return any which have the specified script attached. 
        /// </summary>
        /// <param name="selectedType">The System.Type to search for (must derive from MonoBehaviour)</param>
        /// <returns>The Object id for each prefab who has the script component attached.</returns>
        private static IEnumerable<int> GetPrefabsWhoHaveScript(System.Type selectedType)
        {
            //Get all of the prefabs in the project. 
            var allPrefabs = GetPathToEachPrefabInProject();

            float totalPrefabCount = allPrefabs.Count();
            float currentPrefab = 1;

            //Iterate over all of the prefabs in the project.
            foreach (var prefabPath in allPrefabs)
            {
                //Search could take a long time in a huge project, display a cancelable progress bar to the user in case they get bored. 
                if (EditorUtility.DisplayCancelableProgressBar(string.Format(PROGRESS_BAR_TITLE_FORMAT, selectedType.Name),
                    string.Format(PROGRESS_BAR_MESSAGE_FORMAT, currentPrefab, totalPrefabCount),
                    currentPrefab / totalPrefabCount))
                {
                    //User canceled! Clear the progress bar and return empty! 
                    EditorUtility.ClearProgressBar();
                    yield break;
                }

                //Use the AssetDatabase to load the prefab. 
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) as GameObject;

                //See if the prefab has the component. 
                if (PrefabHasScript(prefab, selectedType))
                {
                    //It did! Return the instance id of this prefab. 
                    yield return prefab.GetInstanceID();
                }                

                //Increment cound so progressbar displays correctly. 
                currentPrefab++;
            }

            //Ensure we clear the progress bar after we successfully complete the entire search. 
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Return the AssetDatabase path to each .prefab in the project. 
        /// </summary>
        private static IEnumerable<string> GetPathToEachPrefabInProject()
        {
            foreach (var prefabGUID in AssetDatabase.FindAssets(PREFAB_FILE_SEARCH_PATTERN))
            {
                yield return AssetDatabase.GUIDToAssetPath(prefabGUID);
            }
        }

        /// <summary>
        /// Search the specified prefabs hierarchy and see if it has the selectedType attached. 
        /// </summary>
        public static bool PrefabHasScript(GameObject prefab, System.Type selectedType)
        {
            //Sanity
            if(prefab == null)
            {
                return false;
            }

            //Now that the prefab is loaded we can see if it has our script component attached. 
            //Search on the root first instead of searching in children, this could provide 
            //a bit of a speedup for big hierarchies. 
            var allComponents = prefab.GetComponents(selectedType);

            if (allComponents.Length != 0)
            {
                return true;
            }
            else
            {
                //We didn't find the component on the root, search in children.. 
                allComponents = prefab.GetComponentsInChildren(selectedType, true);

                if (allComponents != null && allComponents.Length > 0)
                {
                    //One of the children had the component, return the id of root. 
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Make the Project view display the updated selection. 
        /// </summary>
        private static void HighlightResults(int[] ids, System.Type selectedType)
        {
            //Update the selection so the user could drag them into the scene if needed. 
            Selection.instanceIDs = ids;
            //Tried messing around with internal calls via reflection, but the results were not consistent. 
            //Until unity exposes a way to highlight multiselection in the project view, this is the best we can do :(
            EditorGUIUtility.PingObject(Selection.instanceIDs.Last());
            
            //DISABLED THIS TO REDUCE CONSOLE CLUTTER, FEEL FREE TO UNCOMMENT IF YOU LIKE IT!
            //Log the count to the console. 
            //Debug.LogFormat(SUCCESS_MESSAGE_FORMAT, ids.Length, selectedType.Name);
        }
    #endregion Methods
}