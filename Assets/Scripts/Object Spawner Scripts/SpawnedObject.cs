using UnityEngine;
using UnityEngine.Pool;

namespace Object_Spawner_Scripts
{
    public class SpawnedObject : MonoBehaviour
    {
        private IObjectPool<SpawnedObject> _objectPool;         // The IObjectPool stack of spawned objects
        private const string Spawned_Tag = "SpawnedObject";     // A string constant for detecting an object's tag
    
        // On collision detected, if the tag of the object is 
        // the same as the const string value
        // this object will release this object from the pool
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag(Spawned_Tag)) return;
        
            _objectPool.Release(this);
        }

        // This method sets the object pool of this object
        public void SetObjectPool(IObjectPool<SpawnedObject> objectPool)
        {
            _objectPool = objectPool;
        }
    }
}
