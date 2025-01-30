// PlatesConfigEditor.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Fog.Configs
{
    [CustomEditor(typeof(PlatesConfig))]
    public class PlatesConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        
            var config = (PlatesConfig)target;
            if (GUILayout.Button("Sort Plates by Probability"))
            {
                config.SortPlatesByProbability(); 
                EditorUtility.SetDirty(config); 
            }
        }
    }
}
#endif