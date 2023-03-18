using System.Collections;
using Main_Object_Scripts;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Object_Spawner_Scripts
{
    public class ObjectSpawnController : MonoBehaviour
    {
        [SerializeField] private float _minSpawnPosition;               // The minimum position to spawn an object from the pool
        [SerializeField] private float _maxSpawnPosition;               // The maximum position to spawn an object from the pool
        [SerializeField] private float _spawnWaitTime;                  // The interval time between each spawn
        [SerializeField] private int _maxPoolSize;                      // The maximum number of objects to pool
        [SerializeField] private SpawnedObject _spawnedObjectPrefab;    // The object prefab to pool and spawn
        private IObjectPool<SpawnedObject> _spawnedObjectPool;          // The object pool 
        private bool _isSpawning;                                       // Determines if the pool should spawn objects
        private Coroutine _coroutine;                                   // The coroutine reference
    
        private void Awake()
        {
            // Initialise the pool
            _spawnedObjectPool = new ObjectPool<SpawnedObject>(OnSpawnObject, OnRetrieveObject, OnReleaseObject, OnDestroyObject, maxSize: _maxPoolSize);
        }

        private void Start()
        {
            // Subscribe to the action events
            MainObjectController.OnMainObjectMadeFinalDestination += MainObjectHit;
            MainObject.OnMainObjectHit += MainObjectHit;
            
            // Start spawning from the object pool
            _isSpawning = true;
            _coroutine = StartCoroutine(StartSpawning());
        }

        // This coroutine retrieves an object from the pool 
        // at an interval while it is allowed to do so
        private IEnumerator StartSpawning()
        {
            while (_isSpawning)
            {
                _spawnedObjectPool?.Get();
                yield return new WaitForSeconds(_spawnWaitTime);
            }
        }

        // This method subscribes to the action event on the main object
        // so is called when that event is triggered
        private void MainObjectHit()
        {
            _isSpawning = false;
            StopCoroutine(_coroutine);
        }

        // This method is called when an item is created into the pool
        private SpawnedObject OnSpawnObject()
        {
            var spawnedObject = Instantiate(_spawnedObjectPrefab, transform.position, Quaternion.identity);
            spawnedObject.SetObjectPool(_spawnedObjectPool);
            spawnedObject.transform.SetParent(transform);
            return spawnedObject;
        }

        // This method is called when an item is taken from the pool
        private void OnRetrieveObject(SpawnedObject obj)
        {
            var randomXPos = Random.Range(_minSpawnPosition, _maxSpawnPosition);
            var randomZPos = Random.Range(_minSpawnPosition, _maxSpawnPosition);
            var newPosition = new Vector3(randomXPos, transform.position.y, randomZPos);
        
            obj.transform.position = newPosition;
            obj.gameObject.SetActive(true);
        }

        // This method is called when an item is returned to the pool
        private void OnReleaseObject(SpawnedObject obj)
        {
            obj.gameObject.SetActive(false);
        }

        // This method is called if the pool capacity is reached 
        // then any items returned will be destroyed.
        private void OnDestroyObject(SpawnedObject obj)
        {
            Destroy(obj.gameObject);
        }
    }
}
