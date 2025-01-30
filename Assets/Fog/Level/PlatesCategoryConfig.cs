using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Fog.Level
{
    [CreateAssetMenu(menuName = "Content/Configs/Plates/Plates Category Config")]
    public class PlatesCategoryConfig : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<AbstractPlate, float> _platesProbability = new();
        
        public Dictionary<AbstractPlate, float> Plates
        {
            get
            {
                SortPlatesByProbability();
                return _platesProbability;
            }
        }
        
        public void SortPlatesByProbability()
        {
            var sortedEntries = _platesProbability
                .OrderByDescending(kvp => kvp.Value)
                .ToList();
            
            var sum = _platesProbability.Values.Sum();
            _platesProbability.Clear();
            foreach (var entry in sortedEntries)
            {
                _platesProbability.Add(entry.Key, entry.Value / sum);
            }

            Debug.Log("Plates sorted by probability.");
        }
    }
}