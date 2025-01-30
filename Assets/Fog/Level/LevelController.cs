using System.Collections.Generic;
using System.Linq;
using Fog.Configs;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fog.Level
{
    public class LevelController : MonoBehaviour
    {
        private const float SIZE_TO_COORDINATE = 10;
        [SerializeField] private PlatesConfig _platesConfig;
        
        private Dictionary<AbstractPlate, float> _platesPrefabs = new ();
        private Dictionary<Vector2Int, AbstractPlate> _spawnedPlates = new();
        private Dictionary<AbstractPlate, List<Vector2Int>> _platesCoordinates = new();

        private void Start()
        {
            _platesPrefabs = _platesConfig.Plates;
            
            var startingPlates = _platesConfig.StartingPlates;
            TrySpawnPlate(startingPlates, Vector2Int.zero);
        }

        private AbstractPlate GetRandom(Dictionary<AbstractPlate, float> plates)
        {
            var randomValue = Random.Range(0f, 1f);
            var cumulative = 0f;
            foreach (var (platePrefab, probability) in plates)
            {
                cumulative += probability;
                if (randomValue < cumulative)
                {
                    return platePrefab;
                }
            }

            return plates.Keys.Last();
        }

        private void TrySpawnPlate(Dictionary<AbstractPlate, float> plates, Vector2Int coordinates)
        {
            if (_spawnedPlates.ContainsKey(coordinates))
                return;
            
            var platePrefab = GetRandom(plates);
            SpawnPlate(platePrefab, coordinates);
        }

        void SpawnPlate(AbstractPlate platePrefab, Vector2Int coordinates)
        {
            var plate = Instantiate(platePrefab, transform);
            plate.transform.position =
                new Vector3(coordinates.x * SIZE_TO_COORDINATE, 0f, coordinates.y * SIZE_TO_COORDINATE);
            plate.Setup(coordinates, CreateCallback, DestroyCallback);

            var halfX = (int)(plate.transform.lossyScale.x * 0.5f);
            var halfY = (int)(plate.transform.lossyScale.z * 0.5f);
            
            _platesCoordinates.Add(plate, new List<Vector2Int> ());
            for (var i = coordinates.x - halfX; i <= coordinates.x + halfX; i++)
            {
                for (var j = coordinates.y - halfY; j <= coordinates.y + halfY; j++)
                {
                    var plateCoordinate = new Vector2Int(i, j);
                    _spawnedPlates.TryAdd(plateCoordinate, plate);
                    _platesCoordinates[plate].Add(plateCoordinate);
                }
            }
        }

        private void CreateCallback(AbstractPlate plate)
        {
            int halfX = (int)(plate.transform.lossyScale.x * 0.5f);
            int halfY = (int)(plate.transform.lossyScale.z * 0.5f);

            int ceilHalfX = (int)Mathf.Ceil(plate.transform.lossyScale.x * 0.5f);
            int ceilHalfY = (int)Mathf.Ceil(plate.transform.lossyScale.z * 0.5f);

            // Left edge
            int leftX = plate.Coordinates.x - ceilHalfX;
            for (int y = plate.Coordinates.y - halfY; y <= plate.Coordinates.y + halfY; y++)
                TrySpawnPlate(_platesPrefabs, new Vector2Int(leftX, y));

            // Right edge
            int rightX = plate.Coordinates.x + ceilHalfX;
            for (int y = plate.Coordinates.y - halfY; y <= plate.Coordinates.y + halfY; y++)
                TrySpawnPlate(_platesPrefabs, new Vector2Int(rightX, y));

            // Bottom edge
            int bottomY = plate.Coordinates.y - ceilHalfY;
            for (int x = plate.Coordinates.x - halfX; x <= plate.Coordinates.x + halfX; x++)
                TrySpawnPlate(_platesPrefabs, new Vector2Int(x, bottomY));

            // Top edge
            int topY = plate.Coordinates.y + ceilHalfY;
            for (int x = plate.Coordinates.x - halfX; x <= plate.Coordinates.x + halfX; x++)
                TrySpawnPlate(_platesPrefabs, new Vector2Int(x, topY));
        }

        private void DestroyCallback(AbstractPlate plate)
        {
            var plateCoordinates = _platesCoordinates[plate];
            _platesCoordinates.Remove(plate);
            
            foreach (var coordinate in plateCoordinates)
            {
                _spawnedPlates.Remove(coordinate);
            }
            
            Destroy(plate.gameObject);
        }
    }
}