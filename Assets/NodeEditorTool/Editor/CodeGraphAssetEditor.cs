using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CodeGraph.Editor
{
    [CustomEditor(typeof(CodeGraphAsset))]
    public class CodeGraphAssetEditor : UnityEditor.Editor
    {
        //Allows behaviour on double clicking assets in the project window. Behaviour is opening asset.
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int index)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceID);
            if(asset.GetType() == typeof(CodeGraphAsset))
            {
                CodeGraphEditorWindow.Open((CodeGraphAsset)asset);
                return true;
            }

            return false;
        }
        //Creates button in inspector of CodeGraphAsset SerializedObjects that opens CodeGraphEditorWindow
        public override void OnInspectorGUI ()
        {
            if (GUILayout.Button("Open"))
            {
                CodeGraphEditorWindow.Open((CodeGraphAsset)target);
            }
        }

    }
}
