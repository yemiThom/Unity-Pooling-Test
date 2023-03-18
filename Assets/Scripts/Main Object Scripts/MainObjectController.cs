using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main_Object_Scripts
{
    public class MainObjectController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed;          // The movement speed for the main object
        [SerializeField] private Transform _mainObjectPrefab;   // The main object prefab to instantiate
        [SerializeField] private Transform _particleFXprefab;   // The particle system transform
        [SerializeField] private TextAsset _csvFile;            // The csv file to determine the destination positions
        [SerializeField] private AudioSource _audioSource;      // The audio source to play the sound effect

        private Transform _mainObject;                          // The main object transform
        private List<Vector3> _loadedDestinations;              // The list of destinations to move to
        private int _currentDestination = 0;                    // The current destination value to move to
        private bool _canMove;                                  // Determines if the main object can move

        public static event Action OnMainObjectMadeFinalDestination;    // An action even to be triggered when the final destination is reached

        private void Start()
        {
            // Check if there are any missing pieces
            if(_mainObjectPrefab == null) return;
            if(_csvFile == null) return;
            if(_particleFXprefab == null) return;
            if(_audioSource == null) return;

            // Initialise main object
            _mainObject = Instantiate(_mainObjectPrefab, transform.position, Quaternion.identity);
            if(_movementSpeed == 0)
                _movementSpeed = 1;

            // Subscribe to the action event
            MainObject.OnMainObjectHit += MainObjectRemoval;
            
            // Start the main object moving
            _canMove = true;
            ReadCSVFile();
            StartCoroutine(MoveMainObject());
        }

        // This coroutine moves the main object between a list of
        private IEnumerator MoveMainObject()
        {
            while (_canMove)
            {
                // Determine distance of the main object from the current destination
                var distance = (_mainObject.position - _loadedDestinations[_currentDestination]).sqrMagnitude;
                // If the distance is less than 0.1 increment the current destination integer
                if (distance < 0.1f)
                {
                    _currentDestination++;
                }

                // If the current destination integer equals the number of desitinations in the list
                // call the method to remove the main object and invoke the action event
                // then break from the while loop
                if (_currentDestination == _loadedDestinations.Count)
                {
                    MainObjectRemoval();
                    OnMainObjectMadeFinalDestination?.Invoke();
                    yield break;
                }

                // Move the main object to the current destination from the list 
                _mainObject.position = Vector3.MoveTowards(_mainObject.position, _loadedDestinations[_currentDestination], _movementSpeed * Time.deltaTime);
                
                yield return null;
            }
        }

        // This method parses through the csv file
        private void ReadCSVFile()
        {
            // Initialise the list
            _loadedDestinations = new List<Vector3>();

            // Parse through the records in the csv file
            // and placing the values in each cell into a new vector 3
            // which is added to the loaded destinations list
            var records = _csvFile.text.Split("\n");
            for (int i = 0; i < records.Length; i++)
            {
                var values = records[i].Split(",");
                
                if(string.IsNullOrEmpty(values[0])) continue;
                
                _loadedDestinations.Add(new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2])));
            }
        }

        // This method stops the movement loop from continuing
        // then instantiates the particle fx object into the scene
        // and plays the sfx from the audio source
        private void MainObjectRemoval()
        {
            _canMove = false;

            Instantiate(_particleFXprefab, _mainObject.position, Quaternion.identity);
            
            _audioSource.Play();
            _mainObject.gameObject.SetActive(false);
        }
    }
}
