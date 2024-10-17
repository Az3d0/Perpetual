using Mono.Cecil;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CodeGraph.Editor
{
    public class CodeGraphEditorWindow : EditorWindow
    {
        public static void Open(CodeGraphAsset target)
        {
            CodeGraphEditorWindow[] windows = Resources.FindObjectsOfTypeAll<CodeGraphEditorWindow>();
            foreach(var w in windows)
            {
                if(w.currentGraph == target)
                {
                    w.Focus();
                    return;
                }
            }

            CodeGraphEditorWindow window = CreateWindow<CodeGraphEditorWindow>(typeof(CodeGraphEditorWindow), typeof(SceneView));
            window.titleContent = new GUIContent($"{target.name}", EditorGUIUtility.ObjectContent(null, typeof(CodeGraphAsset)).image);
            window.Load(target);
        }

        [SerializeField]
        private CodeGraphAsset m_currentGraph;

        [SerializeField]
        private SerializedObject m_serializedObject;

        [SerializeField]
        private CodeGraphView m_currentView;

        public CodeGraphAsset currentGraph => m_currentGraph;

        //prevents window to flush drawing between compiles and having to reopen currently used CodeGraphAssets
        private void OnEnable()
        {
            if (m_currentGraph != null)
            {
                DrawGraph();
            }
        }

        private void OnGUI()
        {
            if(m_currentGraph != null)
            {
                if (EditorUtility.IsDirty(m_currentGraph))
                {
                    this.hasUnsavedChanges = true;
                }
                else
                {
                    this.hasUnsavedChanges = false;
                }
            }
        }
        public void Load(CodeGraphAsset target)
        {
            m_currentGraph = target;
            DrawGraph();

        }

        private void DrawGraph()
        {
            m_serializedObject = new SerializedObject(m_currentGraph);
            m_currentView = new CodeGraphView(m_serializedObject, this);
            m_currentView.graphViewChanged += OnChange;
            rootVisualElement.Add(m_currentView);
        }

        private GraphViewChange OnChange(GraphViewChange graphViewChange)
        {
            this.hasUnsavedChanges = true;

            //Displays that asset has unsaved changes by adding star next to name in tab
            EditorUtility.SetDirty(m_currentGraph);
            return graphViewChange;
        }
    }
}
