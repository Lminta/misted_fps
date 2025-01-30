#if UNITY_EDITOR
using Fog.Level;
using UnityEditor;
using UnityEngine;

namespace Fog.Configs
{
    [CustomEditor(typeof(PlatesCategoryConfig))]
    public class PlatesCategoryConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        
            var config = (PlatesCategoryConfig)target;
            if (GUILayout.Button("Sort Plates by Probability"))
            {
                config.SortPlatesByProbability(); 
                EditorUtility.SetDirty(config); 
            }
        }
    }
}
#endif