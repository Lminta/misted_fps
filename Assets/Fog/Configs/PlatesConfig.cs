using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Fog.Level;
using UnityEngine;

namespace Fog.Configs
{
    [CreateAssetMenu(menuName = "Content/Configs/Plates/Plates Config")]
    public class PlatesConfig : ScriptableObject
    {
        [SerializeField] private PlatesCategoryConfig _startingPlates;
        [SerializeField] private SerializedDictionary<PlatesCategoryConfig, float> _platesCategoryProbability = new();

        public Dictionary<AbstractPlate, float> StartingPlates => _startingPlates.Plates;
        public Dictionary<AbstractPlate, float> Plates
        {
            get
            {
                SortPlatesByProbability();
                
                var result = new Dictionary<AbstractPlate, float>();
                foreach (var (plateCategory, categoryProbability) in _platesCategoryProbability)
                {
                    var plates = plateCategory.Plates;
                    foreach (var (plate, plateProbability) in plates)
                    {
                        result.Add(plate, plateProbability * categoryProbability);
                    }
                }

                return result;
            }
        }
        
        public void SortPlatesByProbability()
        {
            var sortedEntries = _platesCategoryProbability
                .OrderByDescending(kvp => kvp.Value)
                .ToList();

            var sum = _platesCategoryProbability.Values.Sum();
            _platesCategoryProbability.Clear();
            foreach (var entry in sortedEntries)
            {
                _platesCategoryProbability.Add(entry.Key, entry.Value / sum);
            }

            Debug.Log("Plates sorted by probability.");
        }
    }
}