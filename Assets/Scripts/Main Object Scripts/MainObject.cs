using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main_Object_Scripts
{
    public class MainObject : MonoBehaviour
    {
        public static event Action OnMainObjectHit;         // An action event to be triggered on collision detection
        private const string Spawned_Tag = "SpawnedObject"; // A string constant for detecting an object's tag

        // On collision detected, if the tag of the object is 
        // the same as the const string value
        // this object will invoke the action event for all subscribed
        // methods
        private void OnCollisionEnter(Collision collision)
        {
            if(!collision.transform.CompareTag(Spawned_Tag)) return;
            
            OnMainObjectHit?.Invoke();
        }
    }
}